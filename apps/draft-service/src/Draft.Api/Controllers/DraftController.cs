using Microsoft.AspNetCore.Mvc;
using Draft.Api.Models;
using Draft.Services;

namespace Draft.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class DraftController(IDraftService draftService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var drafts = await draftService.GetAllAsync();
        return Ok(drafts);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var draft = await draftService.GetByIdAsync(id);
        if (draft is null)
            return NotFound();
        return Ok(draft);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDraftRequest request)
    {
        var draft = await draftService.CreateAsync(request.Title, request.Content, request.AuthorId);
        return Created($"/{draft.Id}", draft);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDraftRequest request)
    {
        var draft = await draftService.UpdateAsync(id, request.Title, request.Content);
        if (draft is null)
            return NotFound();
        return Ok(draft);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await draftService.DeleteAsync(id);
        if (!deleted)
            return NotFound();
        return NoContent();
    }
}
