using Microsoft.AspNetCore.Mvc;
using Seismic.UI.Services;

namespace Seismic.UI.Controllers;

[Route("Events")]
public class EventsController(MockSeismicDataService dataService) : Controller
{
    [HttpGet("{id:int}")]
    public IActionResult Details(int id)
    {
        return View("EventDetail", dataService.GetEventDetail(id));
    }
}
