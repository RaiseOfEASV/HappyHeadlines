using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace HappyHeadlines.Monitoring;

public static class TracingExtensions
{
    public static WebApplicationBuilder AddServiceTracing(
        this WebApplicationBuilder builder,
        string serviceName)
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(r => r.AddService(serviceName))
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation();

                var endpoint = builder.Configuration["Otlp:Endpoint"];
                if (!string.IsNullOrEmpty(endpoint))
                    tracing.AddOtlpExporter(o => o.Endpoint = new Uri(endpoint));
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddMeter("happyheadlines.*")
                    .AddPrometheusExporter();
            });

        return builder;
    }
}
