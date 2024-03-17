using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MIL_LIT.Models;

namespace MIL_LIT.Controllers;

public class HomeController : Controller
{
    //private readonly ILogger<HomeController> _logger;

    private  readonly MilLitDbContext _context;

    public HomeController(MilLitDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
