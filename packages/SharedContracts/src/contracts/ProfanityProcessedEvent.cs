namespace SharedContracts.contracts;

public class ProfanityProcessedEvent
{
    public Guid CommentId { get; init; }
    public bool IsApproved { get; init; }
    public string? RejectionReason { get; init; }
}