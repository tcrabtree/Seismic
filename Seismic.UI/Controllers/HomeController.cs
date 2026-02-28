using Microsoft.AspNetCore.Mvc;
using Seismic.UI.Models;

namespace Seismic.UI.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => RedirectToAction("Index", "Sites");

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = HttpContext.TraceIdentifier });
    }
}
