using Microsoft.AspNetCore.Mvc;
using Newsletter.Services.DTOs;
using Newsletter.Services.Services;

namespace Newsletter.Api.Controllers;

[ApiController]
[Route("api/newsletter")]
public class NewsletterController : ControllerBase
{
    private readonly INewsletterService _newsletterService;
    private readonly ILogger<NewsletterController> _logger;

    public NewsletterController(INewsletterService newsletterService, ILogger<NewsletterController> logger)
    {
        _newsletterService = newsletterService;
        _logger = logger;
    }

    [HttpPost("send/daily-digest")]
    public async Task<IActionResult> SendDailyDigest()
    {
        try
        {
            _logger.LogInformation("Manual trigger for daily digest");
            await _newsletterService.SendDailyDigestAsync();
            return Ok(new { message = "Daily digest sent successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send daily digest");
            return StatusCode(500, new { error = "Failed to send daily digest" });
        }
    }

    [HttpGet("history")]
    public async Task<ActionResult<List<NewsletterDto>>> GetHistory([FromQuery] int limit = 50)
    {
        try
        {
            var history = await _newsletterService.GetHistoryAsync(limit);
            return Ok(history);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get newsletter history");
            return StatusCode(500, new { error = "Failed to get newsletter history" });
        }
    }
}
