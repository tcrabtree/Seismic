using Microsoft.AspNetCore.Mvc;
using Seismic.UI.Services;

namespace Seismic.UI.Controllers;

[Route("Sites")]
public class SitesController : Controller
{
    private readonly MockSeismicDataService _dataService;
    public SitesController(MockSeismicDataService dataService)
    {
        _dataService = dataService;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View(_dataService.GetSites());
    }

    [HttpGet("{id:int}/Events")]
    public IActionResult Events(int id, string confidence = "All", bool flaggedOnly = false)
    {
        var model = _dataService.GetSiteEvents(id, confidence, flaggedOnly);
        return View(model);
    }
}
