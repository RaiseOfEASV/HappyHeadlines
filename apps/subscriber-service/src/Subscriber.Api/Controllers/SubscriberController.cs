using Microsoft.AspNetCore.Mvc;
using Subscriber.Services.DTOs;
using Subscriber.Services.Services;

namespace Subscriber.Api.Controllers;

[ApiController]
[Route("api/subscriber")]
public class SubscriberController : ControllerBase
{
    private readonly ISubscriberService _subscriberService;
    private readonly ILogger<SubscriberController> _logger;

    public SubscriberController(ISubscriberService subscriberService, ILogger<SubscriberController> logger)
    {
        _subscriberService = subscriberService;
        _logger = logger;
    }

    [HttpPost("subscribe")]
    public async Task<ActionResult<SubscriberDto>> Subscribe([FromBody] CreateSubscriberDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return BadRequest(new { error = "Email is required" });
        }

        try
        {
            var subscriber = await _subscriberService.SubscribeAsync(dto);
            return Ok(subscriber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to subscribe {Email}", dto.Email);
            return StatusCode(500, new { error = "Failed to subscribe" });
        }
    }

    [HttpDelete("unsubscribe")]
    public async Task<IActionResult> Unsubscribe([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(new { error = "Email is required" });
        }

        try
        {
            var result = await _subscriberService.UnsubscribeAsync(email);
            if (!result)
            {
                return NotFound(new { error = "Subscriber not found or already unsubscribed" });
            }

            return Ok(new { message = "Successfully unsubscribed" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to unsubscribe {Email}", email);
            return StatusCode(500, new { error = "Failed to unsubscribe" });
        }
    }

    [HttpPut("preferences")]
    public async Task<ActionResult<SubscriberDto>> UpdatePreferences(
        [FromQuery] string email,
        [FromBody] UpdatePreferencesDto dto)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(new { error = "Email is required" });
        }

        try
        {
            var subscriber = await _subscriberService.UpdatePreferencesAsync(email, dto);
            if (subscriber == null)
            {
                return NotFound(new { error = "Subscriber not found" });
            }

            return Ok(subscriber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update preferences for {Email}", email);
            return StatusCode(500, new { error = "Failed to update preferences" });
        }
    }

    [HttpGet]
    public async Task<ActionResult<SubscriberDto>> GetByEmail([FromQuery] string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return BadRequest(new { error = "Email is required" });
        }

        try
        {
            var subscriber = await _subscriberService.GetByEmailAsync(email);
            if (subscriber == null)
            {
                return NotFound(new { error = "Subscriber not found" });
            }

            return Ok(subscriber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get subscriber {Email}", email);
            return StatusCode(500, new { error = "Failed to get subscriber" });
        }
    }
}
