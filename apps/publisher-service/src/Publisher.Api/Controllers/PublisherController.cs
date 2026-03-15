using Microsoft.AspNetCore.Mvc;
using Publisher.Api.Models;
using Publisher.Services;
using Publisher.Services.DTOs;

namespace Publisher.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PublisherController : ControllerBase
{
    private readonly IPublisherService _publisherService;
    private readonly ILogger<PublisherController> _logger;

    public PublisherController(IPublisherService publisherService, ILogger<PublisherController> logger)
    {
        _publisherService = publisherService;
        _logger = logger;
    }

    [HttpPost("publish")]
    public async Task<IActionResult> Publish([FromBody] PublishRequest request)
    {
        try
        {
            var dto = new CreatePublicationDto(request.DraftId, request.PublisherId, request.Continent);
            var publication = await _publisherService.PublishDraftAsync(dto);

            return Ok(publication);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid publish request");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing draft {DraftId}", request.DraftId);
            return StatusCode(500, new { error = "Failed to publish draft" });
        }
    }

    [HttpGet("publications/{id:guid}")]
    public async Task<IActionResult> GetPublication(Guid id)
    {
        var publication = await _publisherService.GetPublicationByIdAsync(id);

        if (publication is null)
            return NotFound();

        return Ok(publication);
    }

    [HttpGet("publications")]
    public async Task<IActionResult> GetPublications([FromQuery] Guid? publisherId)
    {
        if (!publisherId.HasValue)
            return BadRequest(new { error = "publisherId query parameter is required" });

        var publications = await _publisherService.GetPublicationsByPublisherAsync(publisherId.Value);

        return Ok(publications);
    }

    [HttpPatch("publications/{id:guid}")]
    public async Task<IActionResult> UpdatePublicationStatus(Guid id, [FromBody] UpdateStatusRequest request)
    {
        var dto = new UpdatePublicationStatusDto(request.Status, request.ArticleId, request.ErrorMessage);
        var publication = await _publisherService.UpdatePublicationStatusAsync(id, dto);

        if (publication is null)
            return NotFound();

        return Ok(publication);
    }

    [HttpPost("publications/{id:guid}/retry")]
    public async Task<IActionResult> RetryPublication(Guid id)
    {
        try
        {
            var publication = await _publisherService.RetryPublicationAsync(id);

            if (publication is null)
                return NotFound();

            return Ok(publication);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Cannot retry publication {PublicationId}", id);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrying publication {PublicationId}", id);
            return StatusCode(500, new { error = "Failed to retry publication" });
        }
    }
}
