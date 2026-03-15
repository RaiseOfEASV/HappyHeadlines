# PublisherService

PublisherService handles the article publishing workflow in HappyHeadlines. It validates drafts, creates publication records, and publishes messages to RabbitMQ for ArticleService to consume.

## Architecture

```
webapp-service → PublisherService → RabbitMQ → ArticleService
                      ↓
                 publication_db
```

## Features

- **Draft Publishing**: Validates drafts and publishes to queue
- **Publication Tracking**: Tracks publication status (Draft, Publishing, Published, Failed)
- **Status Updates**: Callback endpoint for ArticleService to update publication status
- **Retry Mechanism**: Retry failed publications
- **Resilience**: Circuit breaker for DraftService HTTP calls
- **Observability**: Structured logging (Seq) + distributed tracing (Jaeger)

## API Endpoints

### Publish a Draft
```http
POST /api/publisher/publish
Content-Type: application/json

{
  "draftId": "uuid",
  "publisherId": "uuid",
  "continent": "Europe"  // optional
}
```

### Get Publication Status
```http
GET /api/publisher/publications/{id}
```

### List Publications by Publisher
```http
GET /api/publisher/publications?publisherId={uuid}
```

### Update Publication Status (Callback)
```http
PATCH /api/publisher/publications/{id}
Content-Type: application/json

{
  "status": "Published",
  "articleId": "uuid",
  "errorMessage": null
}
```

### Retry Failed Publication
```http
POST /api/publisher/publications/{id}/retry
```

## Database Schema

```sql
CREATE TABLE publications (
    id UUID PRIMARY KEY,
    draft_id UUID NOT NULL,
    article_id UUID NULL,
    publisher_id UUID NOT NULL,
    status VARCHAR(50) NOT NULL,
    title VARCHAR(500) NOT NULL,
    content TEXT NOT NULL,
    continent VARCHAR(50) NULL,
    publish_initiated_at TIMESTAMPTZ NOT NULL,
    publish_completed_at TIMESTAMPTZ NULL,
    error_message TEXT NULL,
    created_at TIMESTAMPTZ NOT NULL,
    updated_at TIMESTAMPTZ NOT NULL
);
```

## Message Published

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

## Quick Start

```bash
# Start RabbitMQ first
cd ../../infra/messaging
docker-compose up -d

# Start PublisherService
cd ../../apps/publisher-service
docker-compose up -d

# View logs
docker-compose logs -f publisher-service

# Stop services
docker-compose down
```

## Configuration

Edit `.env` file:
```
POSTGRES_DB=publisherdb
POSTGRES_USER=publisheruser
POSTGRES_PASSWORD=publisherpass

RABBITMQ_USER=admin
RABBITMQ_PASSWORD=admin
```

## Dependencies

- **PostgreSQL 17**: Publication tracking database
- **RabbitMQ 3**: Message broker
- **DraftService**: HTTP client to validate drafts
- **Seq**: Centralized logging (optional)
- **Jaeger**: Distributed tracing (optional)

## Development

```bash
# Build solution
dotnet build

# Run locally
cd src/Publisher.Api
dotnet run

# Run tests
dotnet test
```

## Observability

- **Logs**: http://localhost:5341 (Seq)
- **Traces**: http://localhost:16686 (Jaeger)
- **Service Port**: http://localhost:5300
- **Swagger**: http://localhost:5300/swagger

## Integration Points

1. **DraftService**: Validates draft exists before publishing
2. **RabbitMQ**: Publishes `PublishArticleCommand` messages
3. **ArticleService**: Consumes commands and sends status callback
