using HappyHeadlines.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;
using Newsletter.Services.DTOs;
using Newsletter.Services.Services;

namespace Newsletter.Services.Consumers;

public class NewSubscriberConsumer : IConsumer<NewSubscriberEvent>
{
    private readonly ISubscriberService _subscriberService;
    private readonly ILogger<NewSubscriberConsumer> _logger;

    public NewSubscriberConsumer(ISubscriberService subscriberService, ILogger<NewSubscriberConsumer> logger)
    {
        _subscriberService = subscriberService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<NewSubscriberEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation("Received NewSubscriberEvent for {Email}", message.Email);

        try
        {
            await _subscriberService.SubscribeAsync(new CreateSubscriberDto
            {
                Email = message.Email,
                Name = message.Name
            });

            _logger.LogInformation("Successfully synced new subscriber {Email} into newsletter database", message.Email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to sync subscriber {Email}", message.Email);
            throw; // Requeue message for retry
        }
    }
}
