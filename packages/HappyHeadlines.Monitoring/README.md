# HappyHeadlines.Monitoring

Shared observability library for all HappyHeadlines services. Provides structured logging via [Serilog](https://serilog.net/) and distributed tracing via [OpenTelemetry](https://opentelemetry.io/), with optional [Seq](https://datalust.co/seq) and [Jaeger](https://www.jaegertracing.io/) backends.

## Usage

### 1. Add a project reference

```xml
<ProjectReference Include="../../../../packages/HappyHeadlines.Monitoring/src/HappyHeadlines.Monitoring.csproj" />
```

Adjust the relative path depth to match your service's location in the repo.

### 2. Wire up in `Program.cs`

```csharp
using HappyHeadlines.Monitoring;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceLogging("your-service-name");  // Serilog — before Build()
builder.AddServiceTracing("your-service-name");  // OpenTelemetry — before Build()
// ...
var app = builder.Build();
app.UseServiceLogging(); // request logging middleware — before MapControllers()
```

`AddServiceLogging` — registers Serilog as the log provider, enriches every event with `ServiceName`, `MachineName`, and `EnvironmentName`.

`AddServiceTracing` — registers OpenTelemetry and auto-instruments incoming HTTP requests, outgoing `HttpClient` calls, and EF Core database queries. Exports spans via OTLP if `Otlp:Endpoint` is configured.

`UseServiceLogging` — adds request logging middleware (one structured event per HTTP request with method, path, status code, duration).

### 3. Configure log levels in `appsettings.json`

```json
"Serilog": {
  "MinimumLevel": {
    "Default": "Information",
    "Override": {
      "Microsoft": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "System": "Warning"
    }
  }
}
```

Use `"Default": "Debug"` in `appsettings.Development.json` to see service-level debug logs locally.

---

## Seq (log UI)

Seq is optional. If `Seq:ServerUrl` is not set the sink is simply not registered.

### Local development

Add to `appsettings.Development.json`:

```json
"Seq": {
  "ServerUrl": "http://localhost:5341",
  "ApiKey": ""
}
```

Leave `ApiKey` empty when running Seq without authentication.

### Docker / production

Pass via environment variables (ASP.NET Core translates `__` → `:`):

```yaml
Seq__ServerUrl: "http://seq:80"
Seq__ApiKey: ${SEQ_API_KEY}
```

### First-run API key setup (one time per environment)

1. Set `SEQ_ADMIN_PASSWORD` in `.env`
2. `docker compose up seq`
3. Open [http://localhost:5341](http://localhost:5341) and log in
4. **Settings → API Keys → Add API Key** (Title: `Ingest`, Permission: `Ingest`)
5. Copy the token into `SEQ_API_KEY=` in `.env`
6. `docker compose up`

---

## Logging in service classes

Inject `ILogger<T>` via the constructor — Serilog powers it transparently.

```csharp
public class MyService(ILogger<MyService> logger)
{
    public async Task DoSomethingAsync(string input)
    {
        logger.LogDebug("Processing input {Input}", input);
        // ...
        logger.LogInformation("Done processing, result: {Result}", result);
    }
}
```

Always use **message templates** (named `{Placeholders}`) rather than string interpolation — this preserves the structured properties in Seq.

---

## Tracing (Jaeger)

Tracing is optional. If `Otlp:Endpoint` is not set no exporter is registered — the SDK still collects spans internally.

Auto-instrumented out of the box:
- **Incoming HTTP requests** — every controller action becomes a root span
- **Outgoing `HttpClient` calls** — propagates trace context to downstream services
- **EF Core queries** — each DB call is a child span with the SQL statement

### Local development

Add to `appsettings.Development.json`:

```json
"Otlp": {
  "Endpoint": "http://localhost:4317"
}
```

### Docker / production

```yaml
Otlp__Endpoint: "http://jaeger:4317"
```

The Jaeger service in `docker-compose.yml` exposes:
- `http://localhost:16686` — Jaeger UI (search and visualise traces)
- `4317` — OTLP gRPC receiver (used by the SDK)

---

## Trace-log correlation

Every log event emitted while a trace is active is automatically enriched with two extra properties:

| Property | Example | Description |
|---|---|---|
| `TraceId` | `4bf92f3577b34da6a3ce929d0e0e4736` | Links the log to its trace in Jaeger |
| `SpanId` | `00f067aa0ba902b7` | Links the log to the specific span |

In Seq, filter by `TraceId = '...'` to see all logs for a single request. Copy the same `TraceId` into Jaeger's search to see the full span tree alongside those logs.
