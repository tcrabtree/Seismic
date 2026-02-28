using Microsoft.AspNetCore.Mvc;
using Seismic.UI.Services;
using Seismic.UI.ViewModels;

namespace Seismic.UI.Controllers;

[Route("Reports")]
public class ReportsController : Controller
{
    private readonly MockSeismicDataService _dataService;
    public ReportsController(MockSeismicDataService dataService)
    {
        _dataService = dataService;
    }
    

    [HttpGet("")]
    public IActionResult Index()
    {
        return View(new ReportsPageViewModel { Sites = _dataService.GetSites() });
    }
}
