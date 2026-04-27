using Comment.Services;

public interface ICreateCommentStrategy
{
    Task<CommentDto> CreateAsync(CreateCommentDto dto);
}