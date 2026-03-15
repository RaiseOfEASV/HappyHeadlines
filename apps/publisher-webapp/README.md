# Webapp Service

Unified React + TypeScript frontend for HappyHeadlines that consolidates draft management functionality with publishing capabilities.

## Features

- **Draft Management**: Create, edit, list, and delete drafts
- **Publishing Workflow**: Publish drafts to the PublisherService
- **Publication Status Tracking**: Real-time status updates for publications
- **Article Preview**: View draft content before publishing
- **OpenTelemetry Instrumentation**: Browser-side tracing with Jaeger integration

## Technology Stack

- React 18.3.1
- TypeScript
- Vite 5.4.10
- React Router 6.28.0
- OpenTelemetry for browser instrumentation
- Nginx for serving and API proxying

## Project Structure

```
webapp-service/
├── src/
│   ├── components/         # React components
│   │   ├── DraftList.tsx
│   │   ├── DraftForm.tsx
│   │   ├── DraftEditor.tsx
│   │   ├── PublishButton.tsx
│   │   ├── PublicationStatus.tsx
│   │   └── PublicationList.tsx
│   ├── services/          # API service layers
│   │   ├── draftService.ts
│   │   └── publisherService.ts
│   ├── types/             # TypeScript interfaces
│   │   └── index.ts
│   ├── pages/             # Route pages
│   │   ├── Home.tsx
│   │   ├── CreateDraft.tsx
│   │   ├── EditDraft.tsx
│   │   ├── PublishDraft.tsx
│   │   └── Publications.tsx
│   ├── tracing.ts         # OpenTelemetry setup
│   ├── App.tsx
│   └── main.tsx
├── Dockerfile
├── nginx.conf
├── docker-compose.yml
└── package.json
```

## API Integration

### Draft Service (http://draft-service:8080)
- `GET /Draft` - List all drafts
- `GET /Draft/{id}` - Get draft by ID
- `POST /Draft` - Create draft
- `PUT /Draft/{id}` - Update draft
- `DELETE /Draft/{id}` - Delete draft

### Publisher Service (http://publisher-service:8080)
- `POST /api/publisher/publish` - Publish draft
- `GET /api/publisher/publications/{id}` - Get publication status
- `GET /api/publisher/publications?publisherId={id}` - List publications

## Development

### Prerequisites
- Node.js 20+
- npm

### Install dependencies
```bash
npm install
```

### Run development server
```bash
npm run dev
```

The app will be available at http://localhost:5173

### Build for production
```bash
npm run build
```

## Docker Deployment

### Build and run with Docker Compose
```bash
docker-compose up --build
```

The service will be available at http://localhost:3000

### Environment Variables
- `VITE_DRAFT_SERVICE_URL` - Draft service base URL (default: http://draft-service:8080)
- `VITE_PUBLISHER_SERVICE_URL` - Publisher service base URL (default: http://publisher-service:8080)
- `VITE_JAEGER_ENDPOINT` - Jaeger OTLP endpoint (default: http://jaeger:4318/v1/traces)

## Nginx Configuration

The nginx configuration proxies API requests:
- `/api/draft` → `http://draft-service:8080/Draft`
- `/api/publisher` → `http://publisher-service:8080/api/publisher`

All other requests serve the React SPA with client-side routing support.

## OpenTelemetry Tracing

The application includes browser-side OpenTelemetry instrumentation that traces:
- Fetch API calls to backend services
- Document load performance
- User navigation flows

Traces are exported to Jaeger via OTLP HTTP.

## Usage

### Creating a Draft
1. Navigate to the home page
2. Click "Create New Draft"
3. Fill in title, content, and author ID
4. Click "Create Draft"

### Publishing a Draft
1. From the draft list, click "Publish" on any draft
2. Enter publisher ID and select continent
3. Click "Publish Draft"
4. Monitor publication status in real-time

### Viewing Publications
1. Navigate to "Publications" in the nav bar
2. Enter a publisher ID
3. Click "Load Publications"
4. View all publications for that publisher
5. Click "View Details" to see publication status

## Notes

- All API calls are automatically traced via OpenTelemetry
- Publication status polling happens every 2 seconds until completion
- Draft deletion requires confirmation
- Simple inline CSS styling for clean, responsive UI
