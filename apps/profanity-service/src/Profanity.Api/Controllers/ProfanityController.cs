using Microsoft.AspNetCore.Mvc;
using Profanity.Services;

namespace Profanity.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProfanityController(IProfanityService profanityService) : ControllerBase
{
    [HttpPost("filter")]
    public async Task<IActionResult> Filter([FromBody] FilterRequest request)
    {
        var filtered = await profanityService.FilterAsync(request.Text);
        return Ok(new { filtered });
    }

    [HttpGet("words")]
    public async Task<IActionResult> GetWords()
    {
        var words = await profanityService.GetWordsAsync();
        return Ok(words);
    }

    [HttpPost("words")]
    public async Task<IActionResult> AddWord([FromBody] AddWordRequest request)
    {
        await profanityService.AddWordAsync(request.Word);
        return Created("", null);
    }
}

public record FilterRequest(string Text);
public record AddWordRequest(string Word);
