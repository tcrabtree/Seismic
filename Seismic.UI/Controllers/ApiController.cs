using Microsoft.AspNetCore.Mvc;
using Seismic.UI.Services;

namespace Seismic.UI.Controllers;

[ApiController]
[Route("api")]
public class ApiController : ControllerBase
{
    private readonly MockSeismicDataService _dataService;
    public ApiController(MockSeismicDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet("sites")]
    public IActionResult GetSites() => Ok(_dataService.GetSites());

    [HttpGet("sites/{id:int}/events")]
    public IActionResult GetSiteEvents(int id, string confidence = "All", bool flaggedOnly = false)
        => Ok(_dataService.GetSiteEvents(id, confidence, flaggedOnly));

    [HttpGet("events/{id:int}")]
    public IActionResult GetEvent(int id) => Ok(_dataService.GetSeismographEventReport(id));

    [HttpPost("events/{id:int}/override")]
    public IActionResult SaveOverride(int id, [FromBody] object payload)
        => Ok(new { eventId = id, saved = true, payload });

    [HttpGet("monitors/{id:int}/health")]
    public IActionResult GetHealth(int id) => Ok(_dataService.GetMonitorHealth(id));

    [HttpPost("reports/generate")]
    public IActionResult GenerateReport([FromBody] object payload)
        => Ok(new { generated = true, payload, generatedAt = DateTime.UtcNow });
}
