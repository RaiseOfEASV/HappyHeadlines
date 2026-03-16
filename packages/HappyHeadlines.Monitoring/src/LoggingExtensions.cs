using Microsoft.AspNetCore.Builder;
using Serilog;
using Serilog.Enrichers.OpenTelemetry;
using Serilog.Formatting.Compact;

namespace HappyHeadlines.Monitoring;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddServiceLogging(
        this WebApplicationBuilder builder,
        string serviceName)
    {
        builder.Host.UseSerilog((context, services, configuration) =>
        {
            var loggerConfig = configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .Enrich.WithProperty("ServiceName", serviceName)
                .Enrich.WithOpenTelemetryTraceId()
                .Enrich.WithOpenTelemetrySpanId()
                .WriteTo.Console(new CompactJsonFormatter());

            var seqUrl = context.Configuration["Seq:ServerUrl"];
            if (!string.IsNullOrEmpty(seqUrl))
            {
                var apiKey = context.Configuration["Seq:ApiKey"];
                loggerConfig.WriteTo.Seq(seqUrl, apiKey: string.IsNullOrEmpty(apiKey) ? null : apiKey);
            }
        });

        return builder;
    }

    public static WebApplication UseServiceLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value ?? string.Empty);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
            };
        });

        app.MapPrometheusScrapingEndpoint("/metrics");

        return app;
    }
}
