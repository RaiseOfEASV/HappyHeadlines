# Newsletter Service

The Newsletter Service is responsible for managing newsletter subscriptions and delivering newsletters to subscribers. It sends breaking news alerts immediately when articles are published and sends a daily digest at 09:00 UTC.

## Architecture

### Tech Stack
- .NET 9.0 with ASP.NET Core
- PostgreSQL 17 for subscriber and newsletter history data
- Redis for caching
- RabbitMQ with MassTransit for consuming published articles
- MailKit for SMTP email delivery
- Polly for resilience (circuit breaker, retry, timeout)
- HappyHeadlines.Monitoring for observability (Seq + Jaeger)

### Port
- **5400** (host) → 8080 (container)

### Dependencies
- PostgreSQL (newsletter-db) on port 5436
- Redis (newsletter-redis) on port 6380
- RabbitMQ (happyheadlines-rabbitmq) on messaging network
- ArticleService for fetching recent articles
- Seq for logging
- Jaeger for distributed tracing

## Database Schema

### Tables

**subscribers**
- `subscriber_id` (UUID, PK)
- `email` (VARCHAR, unique)
- `name` (VARCHAR)
- `is_active` (BOOLEAN)
- `preferences` (JSONB)
- `subscribed_at`, `unsubscribed_at`, `created_at`, `updated_at` (TIMESTAMPTZ)

**newsletter_history**
- `newsletter_id` (UUID, PK)
- `newsletter_type` (VARCHAR) - 'breaking_news' or 'daily_digest'
- `subject` (VARCHAR)
- `content_html` (TEXT)
- `article_ids` (JSONB array)
- `sent_at` (TIMESTAMPTZ)
- `recipient_count` (INTEGER)
- `created_at` (TIMESTAMPTZ)

**newsletter_delivery**
- `delivery_id` (UUID, PK)
- `newsletter_id` (UUID, FK)
- `subscriber_id` (UUID, FK)
- `email` (VARCHAR)
- `status` (VARCHAR) - 'queued', 'sent', 'failed', 'bounced'
- `sent_at`, `opened_at` (TIMESTAMPTZ)
- `error_message` (TEXT)
- `created_at` (TIMESTAMPTZ)

## API Endpoints

### Subscriber Management

#### Subscribe to newsletter
```bash
POST /api/newsletter/subscriber/subscribe
Content-Type: application/json

{
  "email": "user@example.com",
  "name": "John Doe",
  "preferences": {
    "frequency": "daily"
  }
}
```

#### Unsubscribe from newsletter
```bash
DELETE /api/newsletter/subscriber/unsubscribe?email=user@example.com
```

#### Update subscriber preferences
```bash
PUT /api/newsletter/subscriber/preferences?email=user@example.com
Content-Type: application/json

{
  "preferences": {
    "frequency": "weekly",
    "categories": ["technology", "science"]
  }
}
```

#### Get subscriber by email
```bash
GET /api/newsletter/subscriber?email=user@example.com
```

### Newsletter Management

#### Get newsletter history
```bash
GET /api/newsletter/history?limit=50
```

#### Manually trigger daily digest
```bash
POST /api/newsletter/send/daily-digest
```

## Message Consumption

### ArticlePublishedEvent
The service consumes `ArticlePublishedEvent` from RabbitMQ queue `newsletter.article.published`:

```csharp
{
  "ArticleId": "uuid",
  "Name": "Article Title",
  "Content": "Article content...",
  "Timestamp": "2026-03-15T10:00:00Z",
  "Continent": "Europe",
  "IsBreakingNews": true
}
```

When `IsBreakingNews` is true, the service immediately sends a breaking news email to all active subscribers.

## Background Services

### DailyNewsletterScheduler
- Runs daily at 09:00 UTC
- Fetches recent articles from ArticleService
- Generates HTML email with daily digest
- Sends to all active subscribers

## Email Delivery

### Rate Limiting
- Batch size: 50 emails per batch
- Delay between batches: 1 second
- Retry policy: 3 retries with exponential backoff

### SMTP Configuration
Configure SMTP settings in `.env`:
```bash
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-app-password
```

### Email Templates
- **Breaking News**: Red header, single article, immediate delivery
- **Daily Digest**: Blue header, multiple articles, scheduled delivery

## Setup

### Prerequisites
- Docker and Docker Compose
- .NET 9.0 SDK (for local development)
- RabbitMQ messaging network
- Observability network (Seq + Jaeger)

### Environment Variables
Copy `.env.example` to `.env` and configure:

```bash
# Database
POSTGRES_DB=newsletterdb
POSTGRES_USER=newsletteruser
POSTGRES_PASSWORD=newsletterpass

# Redis
REDIS_PASSWORD=redispass

# RabbitMQ
RABBITMQ_USER=admin
RABBITMQ_PASSWORD=admin

# SMTP
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-app-password

# Observability
SEQ_API_KEY=
```

### Running with Docker Compose

```bash
# Build and start all services
docker-compose up -d

# View logs
docker-compose logs -f newsletter-service

# Stop services
docker-compose down

# Stop and remove volumes
docker-compose down -v
```

### Running Locally

```bash
cd src/Newsletter.Api
dotnet restore
dotnet run
```

## Testing

### Subscribe a user
```bash
curl -X POST http://localhost:5400/api/newsletter/subscriber/subscribe \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "name": "Test User"
  }'
```

### Trigger daily digest manually
```bash
curl -X POST http://localhost:5400/api/newsletter/send/daily-digest
```

### Get newsletter history
```bash
curl http://localhost:5400/api/newsletter/history?limit=10
```

### Unsubscribe
```bash
curl -X DELETE "http://localhost:5400/api/newsletter/subscriber/unsubscribe?email=test@example.com"
```

## Integration with Other Services

### ArticleService
- **URL**: Configured via `ArticleServiceUrl` in AppOptions
- **Endpoint**: `GET /api/article/recent?limit=10`
- **Resilience**: Polly circuit breaker + retry + timeout

### RabbitMQ
- **Exchange**: `article-events` (topic exchange)
- **Queue**: `newsletter.article.published`
- **Consumer**: `ArticlePublishedConsumer`

## Observability

### Logging (Seq)
- Structured logging with Serilog
- Log level: Information
- Seq URL: http://localhost:5341

### Tracing (Jaeger)
- OpenTelemetry instrumentation
- Distributed tracing across services
- Jaeger URL: http://localhost:16686

### Metrics
- Email send success/failure rates
- Newsletter delivery status
- Subscriber count
- Background service execution

## Error Handling

### Email Delivery Failures
- Retries: 3 attempts with exponential backoff
- Failed deliveries are logged but don't block other emails
- Status tracked in `newsletter_delivery` table

### Circuit Breaker (ArticleService)
- Opens after 5 consecutive failures
- Break duration: 1 minute
- Graceful degradation: returns empty list

### Consumer Failures
- MassTransit retry: 3 attempts with 5-second intervals
- Dead letter queue for permanent failures

## Project Structure

```
newsletter-service/
├── src/
│   ├── Domain/                 # Minimal domain project
│   ├── Options/                # Configuration options
│   ├── Newsletter.Data/        # EF Core entities and DbContext
│   │   ├── entities/
│   │   │   ├── SubscriberEntity.cs
│   │   │   ├── NewsletterHistoryEntity.cs
│   │   │   └── NewsletterDeliveryEntity.cs
│   │   ├── configuration/
│   │   │   └── NewsletterDbContext.cs
│   │   └── InfrastructureServiceExtensions.cs
│   ├── Newsletter.Services/    # Business logic
│   │   ├── DTOs/
│   │   ├── Messages/
│   │   ├── Clients/
│   │   │   ├── IArticleClient.cs
│   │   │   └── ArticleClient.cs
│   │   ├── Consumers/
│   │   │   └── ArticlePublishedConsumer.cs
│   │   ├── Services/
│   │   │   ├── ISubscriberService.cs
│   │   │   ├── SubscriberService.cs
│   │   │   ├── INewsletterService.cs
│   │   │   ├── NewsletterService.cs
│   │   │   ├── IEmailService.cs
│   │   │   └── EmailService.cs
│   │   ├── BackgroundServices/
│   │   │   └── DailyNewsletterScheduler.cs
│   │   ├── Templates/
│   │   │   └── EmailTemplates.cs
│   │   └── ServicesExtension.cs
│   └── Newsletter.Api/         # API controllers
│       ├── Controllers/
│       │   ├── SubscriberController.cs
│       │   └── NewsletterController.cs
│       └── Program.cs
├── sql/
│   └── V1__InitialCreate.sql
├── Dockerfile
├── docker-compose.yml
├── .env
└── README.md
```

## Development

### Adding a new email template
1. Add template method to `EmailTemplates.cs`
2. Update `NewsletterService` to use new template
3. Test locally before deploying

### Adding a new subscriber preference
1. Update `preferences` JSONB field (no schema changes needed)
2. Update DTOs if needed
3. Update email filtering logic in `NewsletterService`

## Troubleshooting

### No emails are being sent
1. Check SMTP configuration in `.env`
2. Verify SMTP credentials are correct
3. Check logs: `docker-compose logs newsletter-service`
4. Test SMTP connection manually

### Daily digest not running
1. Check background service logs
2. Verify scheduler is running: look for "DailyNewsletterScheduler started" log
3. Check next scheduled time in logs

### Not receiving breaking news
1. Verify RabbitMQ connection
2. Check consumer is registered: `docker-compose logs newsletter-service | grep "ArticlePublishedConsumer"`
3. Verify `IsBreakingNews` flag is set in ArticlePublishedEvent

### ArticleService connection fails
1. Check ArticleServiceUrl configuration
2. Verify article-service is running
3. Check circuit breaker status in logs
4. Test endpoint manually: `curl http://article-service:8080/api/article/recent?limit=10`

## License

Part of the HappyHeadlines project.
