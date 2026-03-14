# Comment Service

A REST API microservice responsible for handling comments on articles in the HappyHeadlines platform.
It communicates directly with the **ProfanityService** to filter comment content before storing it,
and implements a **Circuit Breaker** pattern to remain operational if the ProfanityService goes down.

---

## Architecture

```
Website
  │
  │  POST /api/comment        GET /api/comment/article/{articleId}
  ▼
CommentService  ────────────────────────────────────►  CommentDatabase
  │                                                    (PostgreSQL)
  │  POST /profanity/filter
  ▼
ProfanityService  ──►  ProfanityDatabase
```

### Project Structure

```
comment-service/
├── src/
│   ├── Comment.Api/            # HTTP layer — controllers, Program.cs
│   ├── Comment.Services/       # Business logic, ProfanityClient, Circuit Breaker
│   ├── Comment.Data/           # EF Core DbContext, CommentEntity
│   ├── Options/                # AppOptions configuration binding
│   └── Domain/                 # Domain placeholder
├── sql/
│   └── V1__InitialCreate.sql   # Flyway migration — creates comments table
├── Dockerfile
└── docker-compose.yml
```

---

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/comment/article/{articleId}` | Get all comments for an article |
| `POST` | `/api/comment` | Create a new comment |
| `DELETE` | `/api/comment/{commentId}` | Delete a comment |

### POST `/api/comment` — Request Body

```json
{
  "articleId": "11111111-1111-1111-1111-111111111111",
  "authorId": "22222222-2222-2222-2222-222222222222",
  "content": "This is a great article!"
}
```

### POST `/api/comment` — Response

```json
{
  "commentId": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
  "articleId": "11111111-1111-1111-1111-111111111111",
  "authorId": "22222222-2222-2222-2222-222222222222",
  "content": "This is a great article!",
  "createdAt": "2026-03-14T12:00:00Z"
}
```

---

## Database

**PostgreSQL 17** — single database `comment_db`.

### Schema

```sql
CREATE TABLE comments (
    comment_id  UUID                     NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
    article_id  UUID                     NOT NULL,
    author_id   UUID                     NOT NULL,
    content     TEXT                     NOT NULL,
    created_at  TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);

CREATE INDEX idx_comments_article_id ON comments (article_id);
```

Migrations are handled by **Flyway 10** on startup.

---

## ProfanityService Integration

When a comment is created, the content is sent to the ProfanityService for filtering before being stored:

```
POST http://profanity-service/profanity/filter
Body: { "text": "comment content here" }

Response: { "filtered": "comment content here" }
```

The filtered content is then saved to the database.

---

## Circuit Breaker

The CommentService implements a **Circuit Breaker** pattern using **Polly v8** to ensure fault
isolation (swimlane principle) between the CommentService and ProfanityService.

### How it works

The circuit breaker has 3 states:

```
CLOSED (normal)  ──► too many failures ──►  OPEN (tripped)
                                                │
                                           30 sec break
                                                │
                                                ▼
                                         HALF-OPEN (testing)
                                                │
                                    ┌───────────┴───────────┐
                                 success                  failure
                                    │                        │
                                    ▼                        ▼
                                 CLOSED                    OPEN
```

### Configuration

```csharp
.AddCircuitBreaker(new CircuitBreakerStrategyOptions
{
    SamplingDuration = TimeSpan.FromSeconds(30), // observation window
    FailureRatio = 0.5,                          // open if 50%+ requests fail
    MinimumThroughput = 3,                       // need at least 3 requests before deciding
    BreakDuration = TimeSpan.FromSeconds(30),    // stay open for 30 seconds
})
.AddTimeout(TimeSpan.FromSeconds(5))             // each request times out after 5 seconds
```

### Fallback — Swimlane Isolation

If the circuit is open (ProfanityService is down), the comment is saved **without filtering**
instead of failing the entire request. This ensures the CommentService remains operational
independently of the ProfanityService.

```csharp
try
{
    filteredContent = await profanityClient.FilterAsync(dto.Content);
}
catch (Exception ex)
{
    // Circuit is open or ProfanityService is unavailable
    // Swimlane isolation: save without filtering rather than failing
    logger.LogWarning(ex, "ProfanityService unavailable — saving comment without profanity filtering.");
    filteredContent = dto.Content;
}
```

---

## Running Locally

### Prerequisites
- Docker Desktop

### 1. Start ProfanityService first

```bash
cd apps/profanity-service
docker compose up -d
```

### 2. Start CommentService

```bash
cd apps/comment-service
cp .env.example .env
docker compose up -d
```

### Ports

| Service | Port |
|---------|------|
| Comment API | `http://localhost:5200` |
| Comment Swagger | `http://localhost:5200/swagger` |
| Comment Database | `localhost:5434` |
| Profanity API | `http://localhost:5100` |

---

## Testing

### 1. Test profanity filtering

Add a word to the profanity list via `http://localhost:5100/swagger`:
```json
POST /profanity/words
{ "word": "badword" }
```

Create a comment containing that word via `http://localhost:5200/swagger`:
```json
POST /api/comment
{
  "articleId": "11111111-1111-1111-1111-111111111111",
  "authorId": "22222222-2222-2222-2222-222222222222",
  "content": "This is a badword article!"
}
```

Fetch the comment — the word should be replaced with `*******`.

### 2. Test Circuit Breaker

Stop the ProfanityService:
```bash
cd apps/profanity-service && docker compose stop profanity-api
```

Create a comment — it should still return `201 Created` and be saved without filtering.
Check the CommentService logs for the warning message:
```
Warning: ProfanityService unavailable — saving comment without profanity filtering.
```

---

## Technology Stack

| Component | Technology |
|-----------|-----------|
| Runtime | .NET 9.0 |
| Web Framework | ASP.NET Core 9.0 |
| ORM | Entity Framework Core 9.0 |
| Database | PostgreSQL 17 |
| Migrations | Flyway 10 |
| Circuit Breaker | Polly 8.x |
| API Docs | Swagger / Swashbuckle |
| Container | Docker Compose |