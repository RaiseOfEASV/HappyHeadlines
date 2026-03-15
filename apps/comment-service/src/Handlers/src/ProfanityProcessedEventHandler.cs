using MessageClient.Interfaces;
using Microsoft.Extensions.Logging;
using SharedContracts.contracts;

namespace Handlers;

public class ProfanityProcessedEventHandler(ILogger<CommentCreatedEvent> logger) : IMessageHandler<ProfanityProcessedEvent>
{
    
    public Task Handle(ProfanityProcessedEvent message, CancellationToken cancellationToken)
    {
        if (message.IsApproved)
        {
            Console.WriteLine($"Comment {message.CommentId} was approved.");
            logger.LogInformation($"Comment {message.CommentId} was approved.");
        }
        else
        {
            Console.WriteLine($"Comment {message.CommentId} was rejected. Reason: {message.RejectionReason}");
            logger.LogInformation($"Comment {message.CommentId} was approved.");
        }

        return Task.CompletedTask;
    }
}
