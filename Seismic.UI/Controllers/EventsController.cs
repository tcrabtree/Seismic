using Microsoft.AspNetCore.Mvc;
using Seismic.UI.Services;

namespace Seismic.UI.Controllers;

[Route("Events")]
public class EventsController(ISeismicDataService dataService) : Controller
{
    [HttpGet("{id:int}")]
    public IActionResult Details(int id)
    {
        var model = dataService.GetSeismographEventReport(id);
        if (model is null)
        {
            return NotFound();
        }

        return View("EventDetail", model);
    }
}
