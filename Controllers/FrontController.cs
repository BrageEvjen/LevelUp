using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LevelUp.Models;
using Microsoft.AspNetCore.Authorization;

namespace LevelUp.Controllers;

public class FrontController : Controller
{
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }
    
}