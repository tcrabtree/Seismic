using Microsoft.AspNetCore.Mvc;
using Seismic.UI.Services;
using Seismic.UI.ViewModels;

namespace Seismic.UI.Controllers;

[Route("Reports")]
public class ReportsController(MockSeismicDataService dataService) : Controller
{
    [HttpGet("")]
    public IActionResult Index()
    {
        return View(new ReportsPageViewModel { Sites = dataService.GetSites() });
    }
}
