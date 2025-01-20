using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using LevelUp.Models;
using Microsoft.AspNetCore.Authorization;

namespace LevelUp.Controllers;

public class FrontController : Controller
{
    // AllowAnonymous so users who are not logged in can see it. And IActionResults just makes it so it returns something, and it returns the view
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }
    
}