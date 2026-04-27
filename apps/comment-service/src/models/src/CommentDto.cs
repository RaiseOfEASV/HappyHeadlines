public record CommentDto(Guid CommentId, Guid ArticleId, Guid AuthorId, string Content, DateTime CreatedAt);
public record CreateCommentDto(Guid ArticleId, Guid AuthorId, string Content);