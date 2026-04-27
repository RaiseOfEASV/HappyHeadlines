using Comment.Services;
using Feature.Flags;
using Microsoft.AspNetCore.Mvc;

namespace Comment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController(ICommentService commentService) : ControllerBase
{
    [HttpGet("article/{articleId:guid}")]
    public async Task<IActionResult> GetByArticle(Guid articleId)
    {
        var comments = await commentService.GetByArticleAsync(articleId);
        return Ok(comments);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCommentRequest request)
    {
        var dto = new CreateCommentDto(request.ArticleId, request.AuthorId, request.Content);
        var flag = new ConfigProfanity(request.IsProfanityEnabled);
        var comment = await commentService.CreateAsync(dto,flag);
        return CreatedAtAction(nameof(GetByArticle), new { articleId = comment.ArticleId }, comment);
    }

    [HttpDelete("{commentId:guid}")]
    public async Task<IActionResult> Delete(Guid commentId)
    {
        await commentService.DeleteAsync(commentId);
        return NoContent();
    }
}

public record CreateCommentRequest(Guid ArticleId, Guid AuthorId, string Content, bool IsProfanityEnabled);
