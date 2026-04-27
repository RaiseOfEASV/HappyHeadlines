namespace models;

public interface ICreateCommentStrategy
{
    Task<CommentDto> CreateAsync(CreateCommentDto dto);
}