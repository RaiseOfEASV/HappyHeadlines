using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newsletter.Services.BackgroundServices;
using Newsletter.Services.Clients;
using Newsletter.Services.Consumers;
using Newsletter.Services.Services;
using Options;

namespace Newsletter.Services;

public static class ServicesExtension
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register services
        services.AddScoped<ISubscriberService, SubscriberService>();
        services.AddScoped<INewsletterService, NewsletterService>();
        services.AddScoped<IEmailService, EmailService>();

        // Register HTTP client for ArticleService
        services.AddHttpClient<IArticleClient, ArticleClient>((sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<AppOptions>>().Value;
            client.BaseAddress = new Uri(options.ArticleServiceUrl);
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        // Register background service
        services.AddHostedService<DailyNewsletterScheduler>();

        return services;
    }

    public static IServiceCollection AddMessageBus(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            // Register consumer
            x.AddConsumer<ArticlePublishedConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var options = context.GetRequiredService<IOptions<AppOptions>>().Value.RabbitMq;

                cfg.Host(options.Host, h =>
                {
                    h.Username(options.Username);
                    h.Password(options.Password);
                });

                // Configure endpoint for ArticlePublishedEvent
                cfg.ReceiveEndpoint("newsletter.article.published", e =>
                {
                    e.ConfigureConsumer<ArticlePublishedConsumer>(context);

                    // Retry configuration
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                });
            });
        });

        return services;
    }
}
