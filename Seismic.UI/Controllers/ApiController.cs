using Microsoft.AspNetCore.Mvc;
using Seismic.UI.Services;

namespace Seismic.UI.Controllers;

[ApiController]
[Route("api")]
public class ApiController(MockSeismicDataService dataService) : ControllerBase
{
    [HttpGet("sites")]
    public IActionResult GetSites() => Ok(dataService.GetSites());

    [HttpGet("sites/{id:int}/events")]
    public IActionResult GetSiteEvents(int id, string confidence = "All", bool flaggedOnly = false)
        => Ok(dataService.GetSiteEvents(id, confidence, flaggedOnly));

    [HttpGet("events/{id:int}")]
    public IActionResult GetEvent(int id) => Ok(dataService.GetSeismographEventReport(id));

    [HttpPost("events/{id:int}/override")]
    public IActionResult SaveOverride(int id, [FromBody] object payload)
        => Ok(new { eventId = id, saved = true, payload });

    [HttpGet("monitors/{id:int}/health")]
    public IActionResult GetHealth(int id) => Ok(dataService.GetMonitorHealth(id));

    [HttpPost("reports/generate")]
    public IActionResult GenerateReport([FromBody] object payload)
        => Ok(new { generated = true, payload, generatedAt = DateTime.UtcNow });
}