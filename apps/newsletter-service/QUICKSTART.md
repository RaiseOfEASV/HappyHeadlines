# Newsletter Service - Quick Start Guide

## Prerequisites

1. **Docker and Docker Compose** installed
2. **Observability network** running:
   ```bash
   # Start observability infrastructure (Seq + Jaeger)
   cd /home/kali/bachelors/dls/10/HappyHeadlines/infra/observability
   docker-compose up -d
   ```

3. **Messaging network** running:
   ```bash
   # Start RabbitMQ
   cd /home/kali/bachelors/dls/10/HappyHeadlines/infra/messaging
   docker-compose up -d
   ```

## Quick Start

### 1. Configure Environment

```bash
cd /home/kali/bachelors/dls/10/HappyHeadlines/apps/newsletter-service
cp .env.example .env
# Edit .env to configure SMTP settings
```

### 2. Start the Service

```bash
docker-compose up -d
```

### 3. Verify Running

```bash
# Check service health
curl http://localhost:5400/api/newsletter/history

# View logs
docker-compose logs -f newsletter-service

# Check database
docker exec -it newsletter-db psql -U newsletteruser -d newsletterdb
# Run: \dt to see tables
```

## Testing the Service

### Subscribe a User

```bash
curl -X POST http://localhost:5400/api/newsletter/subscriber/subscribe \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "name": "Test User",
    "preferences": {
      "frequency": "daily",
      "categories": ["technology", "science"]
    }
  }'
```

### Manually Trigger Daily Digest

```bash
curl -X POST http://localhost:5400/api/newsletter/send/daily-digest
```

### View Newsletter History

```bash
curl http://localhost:5400/api/newsletter/history?limit=10 | jq
```

### Unsubscribe a User

```bash
curl -X DELETE "http://localhost:5400/api/newsletter/subscriber/unsubscribe?email=test@example.com"
```

### Get Subscriber Info

```bash
curl "http://localhost:5400/api/newsletter/subscriber?email=test@example.com" | jq
```

## SMTP Configuration Examples

### Gmail (App Password Required)

```bash
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-16-char-app-password
```

To generate Gmail app password:
1. Go to Google Account settings
2. Security > 2-Step Verification
3. App passwords > Generate
4. Use the 16-character password

### SendGrid

```bash
SMTP_HOST=smtp.sendgrid.net
SMTP_PORT=587
SMTP_USERNAME=apikey
SMTP_PASSWORD=your-sendgrid-api-key
```

### Mailtrap (for testing)

```bash
SMTP_HOST=smtp.mailtrap.io
SMTP_PORT=587
SMTP_USERNAME=your-mailtrap-username
SMTP_PASSWORD=your-mailtrap-password
```

## Integration with ArticleService

The newsletter service automatically:
- **Listens for breaking news**: Consumes `ArticlePublishedEvent` from RabbitMQ
- **Sends immediate alerts**: When `IsBreakingNews` is true
- **Fetches recent articles**: For daily digest from ArticleService API

### Test Breaking News Flow

1. Publish an article with breaking news flag (via ArticleService)
2. Newsletter service receives event via RabbitMQ
3. Immediately sends email to all active subscribers

## Background Scheduler

The `DailyNewsletterScheduler` runs automatically:
- **Schedule**: Daily at 09:00 UTC
- **Action**: Fetches recent articles and sends digest to all subscribers
- **Manual trigger**: Use `/api/newsletter/send/daily-digest` endpoint

## Observability

### View Logs in Seq

```
http://localhost:5341
```

Search for:
- `newsletter-service` for all logs
- `SendBreakingNewsAsync` for breaking news sends
- `SendDailyDigestAsync` for digest sends

### View Traces in Jaeger

```
http://localhost:16686
```

Select service: `newsletter-service`

## Database Access

```bash
# Connect to PostgreSQL
docker exec -it newsletter-db psql -U newsletteruser -d newsletterdb

# View subscribers
SELECT * FROM subscribers;

# View newsletter history
SELECT newsletter_id, newsletter_type, subject, recipient_count, sent_at
FROM newsletter_history
ORDER BY sent_at DESC
LIMIT 10;

# View delivery status
SELECT nd.email, nd.status, nd.sent_at, nh.subject
FROM newsletter_delivery nd
JOIN newsletter_history nh ON nd.newsletter_id = nh.newsletter_id
ORDER BY nd.created_at DESC
LIMIT 20;
```

## Troubleshooting

### Emails Not Sending

1. Check SMTP credentials in `.env`
2. View logs: `docker-compose logs newsletter-service | grep "Email"`
3. Check for authentication errors
4. Try with Mailtrap for testing

### Not Receiving Breaking News

1. Verify RabbitMQ is running: `docker ps | grep rabbitmq`
2. Check RabbitMQ management UI: http://localhost:15672 (admin/admin)
3. Verify queue exists: `newsletter.article.published`
4. Check consumer logs: `docker-compose logs newsletter-service | grep "ArticlePublishedConsumer"`

### Daily Digest Not Running

1. Check scheduler logs: `docker-compose logs newsletter-service | grep "DailyNewsletterScheduler"`
2. Verify next run time in logs
3. Manually trigger: `curl -X POST http://localhost:5400/api/newsletter/send/daily-digest`

### ArticleService Connection Failed

1. Verify ArticleService is running
2. Check network connectivity
3. View circuit breaker logs: `docker-compose logs newsletter-service | grep "Circuit breaker"`

## Clean Up

```bash
# Stop services
docker-compose down

# Stop and remove all data
docker-compose down -v
```

## Next Steps

- Configure production SMTP provider (SendGrid, AWS SES, etc.)
- Add more email templates
- Customize subscriber preferences handling
- Set up monitoring alerts for failed deliveries
- Implement email open tracking (if needed)
