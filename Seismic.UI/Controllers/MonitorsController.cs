using Microsoft.AspNetCore.Mvc;
using Seismic.UI.Services;

namespace Seismic.UI.Controllers;

[Route("Monitors")]
public class MonitorsController(MockSeismicDataService dataService) : Controller
{
    [HttpGet("{id:int}/Health")]
    public IActionResult Health(int id)
    {
        return View(dataService.GetMonitorHealth(id));
    }
}
