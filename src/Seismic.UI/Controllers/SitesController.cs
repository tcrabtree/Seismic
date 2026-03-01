using Microsoft.AspNetCore.Mvc;
using Seismic.UI.Services;

namespace Seismic.UI.Controllers;

[Route("Sites")]
public class SitesController(ISeismicDataService dataService) : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View(dataService.GetSites());
    }

    [HttpGet("{id:int}/Events")]
    public IActionResult Events(int id, string confidence = "All", bool flaggedOnly = false)
    {
        var model = dataService.GetSiteEvents(id, confidence, flaggedOnly);
        if (model is null)
        {
            return NotFound();
        }

        return View(model);
    }
}
