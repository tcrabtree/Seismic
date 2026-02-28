using Microsoft.AspNetCore.Mvc;
using Seismic.UI.Services;

namespace Seismic.UI.Controllers;

[Route("Monitors")]
public class MonitorsController : Controller
{
    private readonly MockSeismicDataService _dataService;
    public MonitorsController(MockSeismicDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet("{id:int}/Health")]
    public IActionResult Health(int id)
    {
        return View(_dataService.GetMonitorHealth(id));
    }
}
