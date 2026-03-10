using Article.Services.application_interfaces.ports;
using Microsoft.AspNetCore.Mvc;
using models.articles;

namespace Article.Api.controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController(IArticleService articleService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var articles = await articleService.GetAllArticlesAsync();
        return Ok(articles);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var article = await articleService.GetArticleByIdAsync(id);
        if (article is null)
            return NotFound();
        return Ok(article);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateArticleDto createArticleDto)
    {
        var created = await articleService.CreateArticleAsync(createArticleDto);
        return CreatedAtAction(nameof(GetById), new { id = created.ArticleId }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleDto updateArticleDto)
    {
        var updated = await articleService.UpdateArticleAsync(id, updateArticleDto);
        if (updated is null)
            return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await articleService.DeleteArticleAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}