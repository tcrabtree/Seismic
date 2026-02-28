using Microsoft.AspNetCore.Mvc;
using Seismic.UI.Services;

namespace Seismic.UI.Controllers;

[Route("Events")]
public class EventsController : Controller
{
    private readonly MockSeismicDataService _dataService;
    public  EventsController(MockSeismicDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet("{id:int}")]
    public IActionResult Details(int id)
    {
        return View("EventDetail", _dataService.GetSeismographEventReport(id));
    }
}
