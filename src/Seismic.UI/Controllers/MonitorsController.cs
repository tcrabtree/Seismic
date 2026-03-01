using Microsoft.AspNetCore.Mvc;
using Seismic.UI.Services;

namespace Seismic.UI.Controllers;

[Route("Monitors")]
public class MonitorsController(ISeismicDataService dataService) : Controller
{
    [HttpGet("{id:int}/Health")]
    public IActionResult Health(int id)
    {
        var model = dataService.GetMonitorHealth(id);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }
}
