using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Options;
using Publisher.Services.Clients;

namespace Publisher.Services;

public static class ServicesExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IPublisherService, PublisherService>();

        // Add HTTP client for DraftService
        services.AddHttpClient<IDraftServiceClient, DraftServiceClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<AppOptions>>().Value;
            client.BaseAddress = new Uri(options.DraftServiceUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }

    public static IServiceCollection AddMessageBus(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, cfg) =>
            {
                var options = context.GetRequiredService<IOptions<AppOptions>>().Value.RabbitMq;

                cfg.Host(options.Host, h =>
                {
                    h.Username(options.Username);
                    h.Password(options.Password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });

        return services;
    }
}
