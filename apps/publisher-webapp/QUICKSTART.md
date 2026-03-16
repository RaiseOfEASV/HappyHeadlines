# Webapp Service - Quick Start Guide

## Development Setup

```bash
# Install dependencies
npm install

# Run development server (with hot reload)
npm run dev
# Access at http://localhost:5173
```

## Docker Deployment

```bash
# Build and run
docker-compose up --build

# Access at http://localhost:3000
```

## Architecture Overview

### Frontend Routes
- `/` - Home page (draft list)
- `/create` - Create new draft
- `/edit/:id` - Edit existing draft
- `/publish/:id` - Publish draft workflow
- `/publications` - View publications by publisher

### API Proxying (via Nginx)
- `/api/draft/*` → `http://draft-service:8080/Draft/*`
- `/api/publisher/*` → `http://publisher-service:8080/api/publisher/*`

### Key Components

**DraftList** - Displays all drafts with edit/publish/delete actions
**DraftForm** - Form for creating new drafts
**DraftEditor** - Form for editing existing drafts
**PublishButton** - Modal for publishing a draft
**PublicationStatus** - Real-time status tracking with polling
**PublicationList** - Displays all publications for a publisher

### Services

**draftService.ts** - HTTP client for Draft API
- `fetchDrafts()` - GET /api/draft
- `fetchDraft(id)` - GET /api/draft/{id}
- `createDraft(data)` - POST /api/draft
- `updateDraft(id, data)` - PUT /api/draft/{id}
- `deleteDraft(id)` - DELETE /api/draft/{id}

**publisherService.ts** - HTTP client for Publisher API
- `publishDraft(data)` - POST /api/publisher/publish
- `getPublication(id)` - GET /api/publisher/publications/{id}
- `getPublications(publisherId)` - GET /api/publisher/publications?publisherId={id}

## Observability

### OpenTelemetry Tracing
- Configured in `src/tracing.ts`
- Instruments all fetch() calls
- Traces document load performance
- Exports to Jaeger at `http://jaeger:4318/v1/traces`

### Service Name
- `service.name: webapp-service`

## Common Tasks

### Testing Draft Workflow
1. Create a draft with title, content, and author ID
2. Edit the draft to update content
3. Publish the draft with publisher ID and continent
4. Monitor publication status (auto-polling)
5. View all publications by publisher ID

### Testing Publisher Workflow
1. Navigate to Publications page
2. Enter a publisher ID
3. View all publications for that publisher
4. Click "View Details" to see individual publication status

## Dependencies

### Core
- React 18.3.1
- React Router 6.28.0
- TypeScript 5.6.2
- Vite 5.4.10

### Observability
- @opentelemetry/api 1.9.0
- @opentelemetry/sdk-trace-web 2.6.0
- @opentelemetry/instrumentation-fetch 0.213.0
- @opentelemetry/instrumentation-document-load 0.58.0
- @opentelemetry/exporter-trace-otlp-http 0.213.0

## Environment Variables

Set in docker-compose.yml or .env for development:

```env
VITE_DRAFT_SERVICE_URL=http://draft-service:8080
VITE_PUBLISHER_SERVICE_URL=http://publisher-service:8080
VITE_JAEGER_ENDPOINT=http://jaeger:4318/v1/traces
```

## Troubleshooting

### CORS Issues in Development
- Vite dev server proxies `/api/draft` and `/api/publisher`
- Check `vite.config.ts` proxy configuration

### Production Build Issues
- Ensure all env vars are set at build time (VITE_ prefix)
- Check nginx.conf for correct proxy_pass URLs

### Tracing Not Working
- Verify Jaeger is running and accessible
- Check browser console for OTLP export errors
- Ensure CORS allows trace headers

### API Connection Issues
- Verify draft-service and publisher-service are running
- Check Docker network connectivity
- Inspect nginx logs: `docker logs webapp-service`
