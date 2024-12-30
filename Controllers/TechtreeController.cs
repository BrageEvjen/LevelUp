using System.Diagnostics;
using LevelUp.Data;
using Microsoft.AspNetCore.Mvc;
using LevelUp.Models;
using LevelUp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.Controllers;

public class TechTreeController : Controller
{
    private readonly TechTreeService _techTreeService;
    private readonly ApplicationDbContext _context;  // Add this

    public TechTreeController(
        ILogger<TechTreeController> logger,
        TechTreeService techTreeService,
        ApplicationDbContext context)  // Add this parameter

    {
        _techTreeService = techTreeService;
        _context = context;  // Add this assignment
    }

    public async Task<IActionResult> CompleteNode(int id)
    {
        await _techTreeService.CompleteNode(id);
        return RedirectToAction("Index");
    }
    
    [Authorize]
    public async Task<IActionResult> Index()  // Add async here
    {
        var nodes = await _context.TechTreeNodes
            .Include(n => n.Prerequisites)
            .ToListAsync();
        return View(nodes);
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}