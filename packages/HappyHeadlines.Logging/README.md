# HappyHeadlines.Logging

Shared structured logging library for all HappyHeadlines services. Built on [Serilog](https://serilog.net/) with compact JSON output and optional [Seq](https://datalust.co/seq) sink.

## Usage

### 1. Add a project reference

```xml
<ProjectReference Include="../../../../packages/HappyHeadlines.Logging/src/HappyHeadlines.Logging.csproj" />
```

Adjust the relative path depth to match your service's location in the repo.

### 2. Wire up in `Program.cs`

```csharp
using HappyHeadlines.Logging;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceLogging("your-service-name"); // before Build()
// ...
var app = builder.Build();
app.UseServiceLogging(); // before MapControllers()
```

`AddServiceLogging` — registers Serilog as the log provider, enriches every event with `ServiceName`, `MachineName`, and `EnvironmentName`.

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
