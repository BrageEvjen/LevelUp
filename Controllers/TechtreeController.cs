using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LevelUp.Models;

namespace LevelUp.Controllers;

public class TechtreeController : Controller
{
    private readonly ILogger<TechtreeController> _logger;

    public TechtreeController(ILogger<TechtreeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}