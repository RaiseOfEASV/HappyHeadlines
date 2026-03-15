namespace SharedContracts.contracts;

public class CommentCreatedEvent
{
    public CommentCreatedEvent(Guid commentId, string content)
    
    {
        CommentId = commentId;
        Content = content;
    }

    public Guid CommentId { get; init; }
    public string Content { get; init; }
}