---
name: Implement Missing Microservices for HappyHeadlines
description: Implement PublisherService, NewsletterService, and webapp-service following the architecture diagram
created: 2026-03-15
---

# Implementation Plan: HappyHeadlines Microservices

## Context

The HappyHeadlines project is a distributed news platform with a microservices architecture. Based on the provided architecture diagram, we need to implement three new services and modify one existing service to complete the system:

**Why this change is needed:**
- The architecture diagram shows a complete workflow from article creation → publishing → newsletter distribution
- Currently, only the article storage and draft management pieces exist
- Missing components prevent publishers from publishing articles and subscribers from receiving newsletters

**Current State:**
- ✓ ArticleService exists with multi-region support (MSSQL)
- ✓ DraftService exists for draft management (PostgreSQL)
- ✓ draft-frontend exists for UI
- ✓ CommentService, ProfanityService support services exist
- ✓ Centralized observability infrastructure (Seq + Jaeger)

**Target State:**
- Complete publishing workflow: Draft → PublisherService → ArticleQueue → ArticleService
- Newsletter distribution: ArticleQueue → NewsletterService → Subscribers
- Unified webapp for publishers (consolidates draft + publish functionality)

---

## Services to Implement

### 1. PublisherService (NEW)
**Purpose:** Handles article publishing workflow, validates drafts, publishes to ArticleQueue

**Tech Stack:**
- .NET 9.0 with ASP.NET Core
- PostgreSQL 17 for publication tracking
- RabbitMQ with MassTransit for message publishing
- Flyway for migrations
- HappyHeadlines.Monitoring for observability

**Port:** 5300

**Database Schema:**
```sql
CREATE TABLE publications (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    draft_id UUID NOT NULL,
    article_id UUID NULL,
    publisher_id UUID NOT NULL,
    status VARCHAR(50) NOT NULL,  -- 'Draft', 'Publishing', 'Published', 'Failed'
    title VARCHAR(500) NOT NULL,
    content TEXT NOT NULL,
    publish_initiated_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    publish_completed_at TIMESTAMPTZ NULL,
    error_message TEXT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT now(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT now()
);

CREATE INDEX idx_publications_draft_id ON publications(draft_id);
CREATE INDEX idx_publications_status ON publications(status);
CREATE INDEX idx_publications_publisher_id ON publications(publisher_id);
```

**Key API Endpoints:**
- `POST /api/publisher/publish` - Publish a draft (validates, creates publication, sends to queue)
- `GET /api/publisher/publications/{id}` - Get publication status
- `GET /api/publisher/publications` - List publications with filtering
- `POST /api/publisher/publications/{id}/retry` - Retry failed publication

**Message Published:**
```csharp
public record PublishArticleCommand
{
    public Guid PublicationId { get; init; }
    public Guid DraftId { get; init; }
    public string Title { get; init; }
    public string Content { get; init; }
    public Guid PublisherId { get; init; }
    public List<Guid> AuthorIds { get; init; }
    public string Continent { get; init; }
    public DateTime RequestedAt { get; init; }
}
```

**Integration Points:**
- HTTP client to DraftService (validate draft exists, using Polly resilience)
- RabbitMQ publisher (sends PublishArticleCommand to article-events exchange)
- Callback endpoint for ArticleService to update publication status

**Critical Files to Reference:**
- `/home/kali/bachelors/dls/10/HappyHeadlines/apps/comment-service/src/Comment.Services/src/ProfanityClient.cs` - HTTP client with Polly pattern
- `/home/kali/bachelors/dls/10/HappyHeadlines/apps/draft-service/docker-compose.yaml` - Docker compose pattern
- `/home/kali/bachelors/dls/10/HappyHeadlines/apps/comment-service/sql/V1__InitialCreate.sql` - Flyway migration pattern

---

### 2. NewsletterService (NEW)
**Purpose:** Sends newsletters to subscribers, consumes from ArticleQueue for breaking news, fetches from ArticleService for daily digest

**Tech Stack:**
- .NET 9.0 with ASP.NET Core
- PostgreSQL 17 for subscribers and newsletter history
- RabbitMQ with MassTransit for consuming published articles
- MailKit for SMTP email delivery
- Redis for caching
- BackgroundService for daily newsletter scheduling
- HappyHeadlines.Monitoring for observability

**Port:** 5400

**Database Schema:**
```sql
CREATE TABLE subscribers (
    subscriber_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    email VARCHAR(255) NOT NULL UNIQUE,
    name VARCHAR(255),
    is_active BOOLEAN NOT NULL DEFAULT true,
    preferences JSONB NOT NULL DEFAULT '{}',
    subscribed_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    unsubscribed_at TIMESTAMPTZ NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE newsletter_history (
    newsletter_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    newsletter_type VARCHAR(50) NOT NULL,  -- 'breaking_news' or 'daily_digest'
    subject VARCHAR(500) NOT NULL,
    content_html TEXT NOT NULL,
    article_ids JSONB NOT NULL,
    sent_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    recipient_count INTEGER NOT NULL DEFAULT 0,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);

CREATE TABLE newsletter_delivery (
    delivery_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    newsletter_id UUID NOT NULL REFERENCES newsletter_history(newsletter_id),
    subscriber_id UUID NOT NULL REFERENCES subscribers(subscriber_id),
    email VARCHAR(255) NOT NULL,
    status VARCHAR(50) NOT NULL,  -- 'queued', 'sent', 'failed', 'bounced'
    sent_at TIMESTAMPTZ NULL,
    opened_at TIMESTAMPTZ NULL,
    error_message TEXT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW()
);
```

**Key API Endpoints:**
- `POST /api/newsletter/subscriber/subscribe` - Subscribe to newsletter
- `DELETE /api/newsletter/subscriber/unsubscribe` - Unsubscribe
- `PUT /api/newsletter/subscriber/preferences` - Update preferences
- `POST /api/newsletter/send/daily-digest` - Manual trigger for daily newsletter
- `GET /api/newsletter/history` - Newsletter history

**Message Consumed:**
```csharp
public record ArticlePublishedEvent
{
    public Guid ArticleId { get; init; }
    public string Name { get; init; }
    public string Content { get; init; }
    public DateTime Timestamp { get; init; }
    public string Continent { get; init; }
    public bool IsBreakingNews { get; init; }
}
```

**Integration Points:**
- RabbitMQ consumer (consumes ArticlePublishedEvent for breaking news alerts)
- HTTP client to ArticleService (fetch recent articles for daily digest)
- SMTP email delivery with Polly resilience (retry, circuit breaker, timeout)
- BackgroundService scheduler (daily newsletter at 09:00 UTC)

**Email Delivery:**
- MailKit library for SMTP
- Configurable provider (SendGrid, AWS SES, generic SMTP)
- Rate limiting (100 emails/minute via token bucket)
- HTML email templates (daily digest, breaking news)
- Batch processing (50 emails per batch, 1s delay between batches)

**Critical Files to Reference:**
- `/home/kali/bachelors/dls/10/HappyHeadlines/apps/article-service/src/Article.Services/src/application-services/IArticleLoaderBackgroundservice.cs` - BackgroundService pattern
- `/home/kali/bachelors/dls/10/HappyHeadlines/apps/comment-service/src/Comment.Services/src/ProfanityClient.cs` - HTTP client with Polly
- `/home/kali/bachelors/dls/10/HappyHeadlines/apps/comment-service/docker-compose.yml` - Docker compose with PostgreSQL + Redis

---

### 3. webapp-service (NEW)
**Purpose:** Unified web frontend for publishers to create drafts, edit, and publish articles

**Tech Stack:**
- React 18.3.1 with TypeScript
- Vite 5.4.10 for build tooling
- OpenTelemetry for browser instrumentation
- Nginx for serving static files
- React Router for navigation

**Port:** 3000

**Features:**
- Draft management UI (create, edit, list, delete drafts)
- Article publishing workflow (publish button → calls PublisherService)
- Publication status tracking
- Article preview
- Integration with DraftService API
- Integration with PublisherService API

**Key Pages/Routes:**
- `/` - Dashboard (list drafts)
- `/drafts/new` - Create new draft
- `/drafts/:id/edit` - Edit draft
- `/drafts/:id/publish` - Publish confirmation
- `/publications` - Publication history
- `/publications/:id` - Publication status

**API Integration:**
- DraftService: `http://draft-service:PORT` (CRUD operations)
- PublisherService: `http://publisher-service:5300` (publish, status)

**Observability:**
- OpenTelemetry browser instrumentation
- Fetch API tracing
- Document load tracing
- Jaeger integration via OTLP HTTP exporter

**Critical Files to Reference:**
- `/home/kali/bachelors/dls/10/HappyHeadlines/apps/draft-frontend/` - React + TypeScript + Vite structure
- `/home/kali/bachelors/dls/10/HappyHeadlines/apps/draft-frontend/Dockerfile` - Nginx serving pattern

---

### 4. ArticleService Modifications (EXISTING SERVICE)
**Purpose:** Add RabbitMQ consumer to create articles when PublishArticleCommand is received

**Changes Required:**
1. Add MassTransit NuGet packages to Article.Services project
2. Create consumer: `PublishArticleConsumer.cs`
3. Configure MassTransit in `Program.cs`
4. Add callback HTTP client to PublisherService (update publication status)

**New Consumer:**
```csharp
public class PublishArticleConsumer : IConsumer<PublishArticleCommand>
{
    private readonly IArticleService _articleService;
    private readonly IPublisherClient _publisherClient;
    private readonly ILogger<PublishArticleConsumer> _logger;

    public async Task Consume(ConsumeContext<PublishArticleCommand> context)
    {
        var command = context.Message;

        try
        {
            // Create article in database
            var article = await _articleService.CreateArticleAsync(new CreateArticleDto
            {
                Name = command.Title,
                Content = command.Content,
                Continent = command.Continent,
                AuthorIds = command.AuthorIds
            });

            // Notify PublisherService of success
            await _publisherClient.UpdatePublicationStatusAsync(
                command.PublicationId,
                "Published",
                article.ArticleId);

            // Publish ArticlePublishedEvent for NewsletterService
            await context.Publish(new ArticlePublishedEvent
            {
                ArticleId = article.ArticleId,
                Name = article.Name,
                Content = article.Content,
                Continent = command.Continent,
                Timestamp = DateTime.UtcNow,
                IsBreakingNews = DetermineBreakingNews(command)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create article");

            // Notify PublisherService of failure
            await _publisherClient.UpdatePublicationStatusAsync(
                command.PublicationId,
                "Failed",
                null,
                ex.Message);

            throw; // Requeue message for retry
        }
    }
}
```

**Files to Modify:**
- `/home/kali/bachelors/dls/10/HappyHeadlines/apps/article-service/src/Article.Services/src/ServicesExtension.cs` - Add MassTransit registration
- `/home/kali/bachelors/dls/10/HappyHeadlines/apps/article-service/src/Article.Api/Program.cs` - Configure MassTransit
- `/home/kali/bachelors/dls/10/HappyHeadlines/apps/article-service/docker-compose.yaml` - Add RabbitMQ dependency
- `/home/kali/bachelors/dls/10/HappyHeadlines/apps/article-service/src/Options/src/AppOptions.cs` - Add RabbitMQ configuration

**New Files to Create:**
- `/home/kali/bachelors/dls/10/HappyHeadlines/apps/article-service/src/Article.Services/src/Consumers/PublishArticleConsumer.cs`
- `/home/kali/bachelors/dls/10/HappyHeadlines/apps/article-service/src/Article.Services/src/PublisherClient.cs`
- `/home/kali/bachelors/dls/10/HappyHeadlines/apps/article-service/src/Article.Services/src/Messages/` (message contracts)

---

## RabbitMQ Infrastructure

**Components:**
- RabbitMQ 3 Management Alpine
- Management UI on port 15672
- AMQP on port 5672

**Exchange and Queue Configuration:**
- Exchange: `article-events` (topic exchange)
- Queue 1: `article.publish` (consumed by ArticleService)
  - Routing key: `article.publish.command`
- Queue 2: `newsletter.article.published` (consumed by NewsletterService)
  - Routing key: `article.published.event`
- Dead Letter Queue: `article.publish.dlq`

**Docker Compose (Shared):**
```yaml
rabbitmq:
  image: rabbitmq:3-management-alpine
  container_name: happyheadlines-rabbitmq
  ports:
    - "5672:5672"
    - "15672:15672"
  environment:
    RABBITMQ_DEFAULT_USER: ${RABBITMQ_USER}
    RABBITMQ_DEFAULT_PASS: ${RABBITMQ_PASSWORD}
  volumes:
    - rabbitmq-data:/var/lib/rabbitmq
  healthcheck:
    test: rabbitmq-diagnostics -q ping
    interval: 10s
    timeout: 5s
    retries: 5
  networks:
    - happyheadlines-network
```

**Message Flow:**
1. webapp-service → PublisherService: `POST /api/publisher/publish`
2. PublisherService → RabbitMQ: `PublishArticleCommand` (article.publish.command)
3. RabbitMQ → ArticleService: Consumer processes command
4. ArticleService → Database: Create article
5. ArticleService → RabbitMQ: `ArticlePublishedEvent` (article.published.event)
6. RabbitMQ → NewsletterService: Consumer sends breaking news newsletter
7. ArticleService → PublisherService: `PATCH /api/publisher/publications/{id}` (status callback)

---

## Implementation Sequence

### Phase 1: Infrastructure Setup (Week 1)
1. Create RabbitMQ docker-compose in `/infra/messaging/`
2. Create shared Docker network `happyheadlines-network`
3. Test RabbitMQ connectivity and management UI

### Phase 2: PublisherService (Week 1-2)
4. Create solution structure (`publisher-service/`)
5. Set up PostgreSQL + Flyway migrations
6. Implement data layer (PublicationEntity, DbContext, repositories)
7. Implement messaging layer (RabbitMQ publisher with MassTransit)
8. Implement service layer (PublisherService, DraftServiceClient)
9. Implement API layer (PublisherController, DTOs)
10. Configure observability (HappyHeadlines.Monitoring)
11. Create Dockerfile and docker-compose.yml
12. Integration testing

### Phase 3: ArticleService Consumer (Week 2)
13. Add MassTransit packages to ArticleService
14. Create message contracts (PublishArticleCommand, ArticlePublishedEvent)
15. Implement PublishArticleConsumer
16. Implement PublisherClient (HTTP client for status callback)
17. Update Program.cs with MassTransit configuration
18. Update docker-compose.yml with RabbitMQ dependency
19. Test end-to-end: PublisherService → Queue → ArticleService

### Phase 4: NewsletterService (Week 3)
20. Create solution structure (`newsletter-service/`)
21. Set up PostgreSQL + Redis + Flyway migrations
22. Implement data layer (Subscriber, NewsletterHistory, NewsletterDelivery entities)
23. Implement email layer (EmailClient with MailKit, templates)
24. Implement messaging layer (ArticlePublishedConsumer)
25. Implement service layer (NewsletterService, ArticleClient, SubscriberService)
26. Implement API layer (SubscriberController, NewsletterController)
27. Implement BackgroundService (DailyNewsletterScheduler)
28. Configure observability
29. Create Dockerfile and docker-compose.yml
30. Integration testing

### Phase 5: webapp-service (Week 4)
31. Create React + TypeScript + Vite project structure
32. Set up React Router
33. Implement Draft Management pages (list, create, edit, delete)
34. Implement Publishing workflow (publish button, status tracking)
35. Integrate with DraftService API
36. Integrate with PublisherService API
37. Add OpenTelemetry browser instrumentation
38. Create Dockerfile with Nginx
39. Create docker-compose.yml
40. End-to-end testing

### Phase 6: Integration & Testing (Week 4)
41. Test complete workflow: Draft → Publish → ArticleQueue → Article creation
42. Test newsletter workflow: Article published → Newsletter sent
43. Test daily digest scheduler
44. Verify observability (Seq logs, Jaeger traces)
45. Load testing (newsletter batch sending)
46. Documentation (README files, API documentation)

---

## Verification Plan

### End-to-End Workflow Testing:

**Test 1: Article Publishing Flow**
1. Start all services (webapp, publisher, article-service, rabbitmq)
2. Create a draft via webapp-service
3. Click "Publish" button → calls PublisherService
4. Verify PublisherService creates publication record (status: Publishing)
5. Verify RabbitMQ receives PublishArticleCommand
6. Verify ArticleService consumer creates article in database
7. Verify ArticleService publishes ArticlePublishedEvent
8. Verify PublisherService publication status updated to "Published"

**Test 2: Newsletter Flow**
1. Subscribe email via NewsletterService API
2. Publish article with IsBreakingNews=true
3. Verify NewsletterService consumer receives ArticlePublishedEvent
4. Verify breaking news email sent to subscriber
5. Check newsletter_delivery table for status "sent"

**Test 3: Daily Digest**
1. Wait for scheduled time (09:00 UTC) or manually trigger
2. Verify NewsletterService fetches recent articles from ArticleService
3. Verify daily digest email generated and sent
4. Check newsletter_history table

**Test 4: Observability**
1. Publish an article end-to-end
2. Check Seq (http://localhost:5341) for structured logs from all services
3. Check Jaeger (http://localhost:16686) for distributed trace spanning webapp → publisher → queue → article
4. Verify correlation IDs propagate through entire workflow

---

## Critical Design Decisions

### 1. Message Queue: RabbitMQ with MassTransit
- **Rationale:** Industry standard for .NET, excellent abstraction, simpler than Kafka for this use case
- **Trade-off:** Additional infrastructure component to manage
- **Pattern:** Command messages (PublishArticleCommand) and Event messages (ArticlePublishedEvent)

### 2. Publishing Workflow: Draft-First
- **Rationale:** Separates draft state from published state, allows review before publishing
- **Flow:** Draft → PublisherService validation → Queue → Article creation
- **Benefit:** Atomic publishing with status tracking and retry capability

### 3. Email Delivery: SMTP with MailKit
- **Rationale:** Flexible, no vendor lock-in, works with any SMTP provider
- **Resilience:** Polly retry + circuit breaker + timeout
- **Rate Limiting:** Token bucket algorithm (100 emails/minute)

### 4. Newsletter Scheduling: BackgroundService
- **Rationale:** Built-in .NET feature, no external dependencies (vs Hangfire/Quartz)
- **Schedule:** Daily at 09:00 UTC
- **Trade-off:** Less sophisticated than Hangfire, but simpler

### 5. Unified Webapp
- **Rationale:** User request to consolidate draft-frontend functionality
- **Benefit:** Single UI for publishers, easier maintenance
- **Pattern:** React SPA with API integration to multiple backend services

---

## Architectural Patterns Used

1. **Layered Architecture:** API → Services → Data (all .NET services)
2. **Message-Driven:** Async communication via RabbitMQ for publishing workflow
3. **HTTP REST:** Synchronous communication for API calls
4. **CQRS-lite:** Separate read (ArticleService GET) and write (ArticleService Consumer) paths
5. **Circuit Breaker:** Polly resilience for all HTTP clients
6. **Observability-First:** Structured logging (Seq) + Distributed tracing (Jaeger) integrated from start
7. **Database per Service:** Each service owns its data (publisher_db, newsletter_db, article_db, etc.)
8. **Event Sourcing-lite:** Publication tracking for audit trail and status management

---

## Files and Directories

### New Directories to Create:
```
/home/kali/bachelors/dls/10/HappyHeadlines/
├── apps/
│   ├── publisher-service/          # PublisherService implementation
│   │   ├── src/
│   │   ├── sql/
│   │   ├── Dockerfile
│   │   ├── docker-compose.yml
│   │   └── PublisherService.sln
│   ├── newsletter-service/         # NewsletterService implementation
│   │   ├── src/
│   │   ├── sql/
│   │   ├── Dockerfile
│   │   ├── docker-compose.yml
│   │   └── NewsletterService.sln
│   └── webapp-service/             # Unified React webapp
│       ├── src/
│       ├── public/
│       ├── Dockerfile
│       ├── docker-compose.yml
│       └── package.json
└── infra/
    └── messaging/                  # Shared RabbitMQ infrastructure
        └── docker-compose.yml
```

### Files to Modify:
- `/home/kali/bachelors/dls/10/HappyHeadlines/apps/article-service/src/Article.Services/src/ServicesExtension.cs`
- `/home/kali/bachelors/dls/10/HappyHeadlines/apps/article-service/src/Article.Api/Program.cs`
- `/home/kali/bachelors/dls/10/HappyHeadlines/apps/article-service/docker-compose.yaml`
- `/home/kali/bachelors/dls/10/HappyHeadlines/apps/article-service/src/Options/src/AppOptions.cs`

---

## Dependencies (NuGet Packages)

### PublisherService:
- MassTransit (8.3.4)
- MassTransit.RabbitMQ (8.3.4)
- Npgsql.EntityFrameworkCore.PostgreSQL (9.0.13)
- Polly (8.5.2)
- Swashbuckle.AspNetCore (10.1.3)

### NewsletterService:
- MassTransit (8.3.4)
- MassTransit.RabbitMQ (8.3.4)
- MailKit (4.9.0)
- Npgsql.EntityFrameworkCore.PostgreSQL (9.0.13)
- Microsoft.Extensions.Caching.StackExchangeRedis (9.0.3)
- Polly (8.5.2)

### ArticleService (additions):
- MassTransit (8.3.4)
- MassTransit.RabbitMQ (8.3.4)
- Polly (8.5.2)

### webapp-service:
- react (18.3.1)
- react-router-dom (latest)
- @opentelemetry/api (latest)
- @opentelemetry/sdk-trace-web (latest)

---

## Estimated Timeline
- **Week 1:** RabbitMQ setup + PublisherService foundation
- **Week 2:** Complete PublisherService + ArticleService consumer integration
- **Week 3:** NewsletterService implementation
- **Week 4:** webapp-service + Integration testing
- **Total:** 4 weeks for full implementation

---

## Success Criteria
✓ Publishers can create drafts in webapp-service
✓ Publishers can publish drafts via PublisherService
✓ Articles are created in ArticleService via queue consumer
✓ Newsletter subscribers receive breaking news alerts
✓ Daily digest newsletter sent at scheduled time
✓ Complete observability (logs in Seq, traces in Jaeger)
✓ All services containerized with Docker Compose
✓ End-to-end workflow tested and documented
