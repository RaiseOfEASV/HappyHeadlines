using Comment.Data.configuration;
using Comment.Data.entities;
using MessageClient.Interfaces;
using SharedContracts.contracts;

namespace models;

public class CreateCommentWithoutProfanity : ICreateCommentStrategy
{
    private readonly CommentDbContext _db;
    private readonly IMessageClient _messageClient;

    public CreateCommentWithoutProfanity(
        CommentDbContext db,
        IMessageClient messageClient)
    {
        _db = db;
        _messageClient = messageClient;
    }

    public async Task<CommentDto> CreateAsync(CreateCommentDto dto)
    {
        var entity = new CommentEntity
        {
            CommentId = Guid.NewGuid(),
            ArticleId = dto.ArticleId,
            AuthorId = dto.AuthorId,
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow
        };

        await _db.Comments.AddAsync(entity);
        await _db.SaveChangesAsync();

        await _messageClient.PublishAsync(
            new CommentCreatedEvent(entity.CommentId, dto.Content));

        return new CommentDto(entity.CommentId, entity.ArticleId, entity.AuthorId, entity.Content, entity.CreatedAt);
    }
}