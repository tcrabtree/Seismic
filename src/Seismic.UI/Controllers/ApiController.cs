using Microsoft.AspNetCore.Mvc;
using Seismic.UI.Models;
using Seismic.UI.Services;

namespace Seismic.UI.Controllers;

[ApiController]
[Route("api")]
public class ApiController(ISeismicDataService dataService) : ControllerBase
{
    [HttpGet("sites")]
    public IActionResult GetSites() => Ok(dataService.GetSites());

    [HttpGet("sites/{id:int}/events")]
    public IActionResult GetSiteEvents(int id, string confidence = "All", bool flaggedOnly = false)
    {
        var siteEvents = dataService.GetSiteEvents(id, confidence, flaggedOnly);
        return siteEvents is null ? NotFound(new { message = "Site not found." }) : Ok(siteEvents);
    }

    [HttpGet("events/{id:int}")]
    public IActionResult GetEvent(int id)
    {
        var seismicEvent = dataService.GetSeismographEventReport(id);
        return seismicEvent is null ? NotFound(new { message = "Event not found." }) : Ok(seismicEvent);
    }

    [HttpPost("events/{id:int}/override")]
    public IActionResult SaveOverride(int id, [FromBody] SaveOverrideRequest payload)
    {
        if (dataService.GetSeismographEventReport(id) is null)
        {
            return NotFound(new { message = "Event not found." });
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        return Ok(new
        {
            eventId = id,
            saved = true,
            decision = payload.Decision,
            notes = payload.Notes,
            savedAt = DateTime.UtcNow
        });
    }

    [HttpGet("monitors/{id:int}/health")]
    public IActionResult GetHealth(int id)
    {
        var health = dataService.GetMonitorHealth(id);
        return health is null ? NotFound(new { message = "Monitor not found." }) : Ok(health);
    }

    [HttpPost("reports/generate")]
    public IActionResult GenerateReport([FromBody] GenerateReportRequest payload)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (!dataService.GetSites().Any(s => s.Id == payload.SiteId))
        {
            return NotFound(new { message = "Site not found." });
        }

        if (payload.EndDate < payload.StartDate)
        {
            return BadRequest(new { message = "End date must be on or after start date." });
        }

        return Ok(new
        {
            generated = true,
            payload,
            generatedAt = DateTime.UtcNow
        });
    }
}
