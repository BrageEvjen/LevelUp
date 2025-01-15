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
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TechTreeController(
        ILogger<TechTreeController> logger,
        TechTreeService techTreeService,
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        _techTreeService = techTreeService;
        _context = context;
        _userManager = userManager;
    }
    
    [Authorize]
    public async Task<IActionResult> Index(int treeId = 1)
    {
        var nodes = await _context.TechTreeNodes
            .Include(n => n.Prerequisites)
            .Where(n => n.TechTreeId == treeId)
            .ToListAsync();
        
        ViewBag.CurrentTreeId = treeId;
        ViewBag.AvailableTrees = await _context.TechTreeNodes
            .Select(n => n.TechTreeId)
            .Distinct()
            .ToListAsync();
            
        return View(nodes);
    }

    [HttpPost]
    public async Task<JsonResult> CompleteNode(int id)
    {
        var success = await _techTreeService.CompleteNode(id);
        return Json(new { success });
    }    
    
    [HttpGet]
    public async Task<JsonResult> GetNodeStatuses(int treeId)
    {
        var nodes = await _context.TechTreeNodes
            .Include(n => n.Prerequisites)
            .Where(n => n.TechTreeId == treeId)
            .Select(n => new { 
                id = n.Id, 
                status = n.Status 
            })
            .ToListAsync();
        return Json(nodes);
    }
    
    [HttpGet]
    public async Task<JsonResult> GetUserProgress()
    {
        var user = await _userManager.GetUserAsync(User);
        return Json(new { 
            level = user.CurrentLevel,
            currentXP = user.CurrentXP,
            xpToNext = user.XPToNextLevel,
            leveledUp = user.AddXP(0)
        });
    }
    
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}