namespace Comment.Services;

public interface ICommentCacheService
{
    Task<IEnumerable<CommentDto>?> GetAsync(Guid articleId);
    Task SetAsync(Guid articleId, IEnumerable<CommentDto> comments);
    Task InvalidateAsync(Guid articleId);
}
