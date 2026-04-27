using Comment.Data.configuration;
using Comment.Data.entities;
using Comment.Services;
using MessageClient.Interfaces;
using SharedContracts.contracts;

public class CreateCommentWithProfanity : ICreateCommentStrategy
{
    private readonly IProfanityClient _profanityClient;
    private readonly CommentDbContext _db;
    private readonly IMessageClient _messageClient;

    public CreateCommentWithProfanity(
        IProfanityClient profanityClient,
        CommentDbContext db,
        IMessageClient messageClient)
    {
        _profanityClient = profanityClient;
        _db = db;
        _messageClient = messageClient;
    }

    public async Task<CommentDto> CreateAsync(CreateCommentDto dto)
    {
        var filtered = await _profanityClient.FilterAsync(dto.Content);

        var entity = new CommentEntity
        {
            CommentId = Guid.NewGuid(),
            ArticleId = dto.ArticleId,
            AuthorId = dto.AuthorId,
            Content = filtered,
            CreatedAt = DateTime.UtcNow
        };

        await _db.Comments.AddAsync(entity);
        await _db.SaveChangesAsync();

        await _messageClient.PublishAsync(
            new CommentCreatedEvent(entity.CommentId, dto.Content));

        return new CommentDto(entity.CommentId, entity.ArticleId, entity.AuthorId, entity.Content, entity.CreatedAt);
    }
}