using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Options;
using Publisher.Services.Clients;
using Publisher.Services.Consumers;

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

        // Add HTTP client for ArticleService
        services.AddHttpClient<IArticleServiceClient, ArticleServiceClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<AppOptions>>().Value;
            client.BaseAddress = new Uri(options.ArticleServiceUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }

    public static IServiceCollection AddMessageBus(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<PublishArticleCommandConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var options = context.GetRequiredService<IOptions<AppOptions>>().Value.RabbitMq;

                cfg.Host(options.Host, h =>
                {
                    h.Username(options.Username);
                    h.Password(options.Password);
                });

                cfg.ReceiveEndpoint("publisher.publish-article", e =>
                {
                    e.ConfigureConsumer<PublishArticleCommandConsumer>(context);
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });
            });
        });

        return services;
    }
}
