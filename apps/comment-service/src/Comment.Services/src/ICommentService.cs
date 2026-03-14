namespace Comment.Services;

public interface ICommentService
{
    Task<IEnumerable<CommentDto>> GetByArticleAsync(Guid articleId);
    Task<CommentDto> CreateAsync(CreateCommentDto dto);
    Task DeleteAsync(Guid commentId);
}

public record CommentDto(Guid CommentId, Guid ArticleId, Guid AuthorId, string Content, DateTime CreatedAt);
public record CreateCommentDto(Guid ArticleId, Guid AuthorId, string Content);
