using Feature.Flags;

namespace Comment.Services;

public interface ICommentService
{
    Task<IEnumerable<CommentDto>> GetByArticleAsync(Guid articleId);
    Task<CommentDto> CreateAsync(CreateCommentDto dto,ConfigProfanity flags);
    Task DeleteAsync(Guid commentId);
}


