using LevelUp.Data;
using LevelUp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        ILogger<HomeController> logger)
    {
        _context = context;
        _userManager = userManager;
        _logger = logger;
    }

    public IActionResult Privacy()
    {
        return View();
    }

    
    [Authorize]
    public async Task<IActionResult> Index()
    {
            // Getting the curretnly logged in user
            var user = await _userManager.GetUserAsync(User);
            // Gets the quest that is assigned to the current user
            var quest = await _context.DailyQuests
                .FirstOrDefaultAsync(d => d.UserId == user.Id && d.QuestName == "Make Your Bed");

            // If no quest is found then it generates a new quest
            if (quest == null)
            {
                quest = new DailyQuest
                {
                    UserId = user.Id,
                    QuestName = "Make Your Bed",
                    CurrentStreak = 0,
                    LastCompletedDate = DateTime.MinValue,
                    CompletedToday = false
                };
                _context.DailyQuests.Add(quest);
                await _context.SaveChangesAsync();
            }
            // Or if there is a quest 
            else
            {
                // Reset CompletedToday if it's a new day
                if (quest.LastCompletedDate.Date < DateTime.Today)
                {
                    quest.CompletedToday = false;
                    // Reset streak if a day was missed
                    if (quest.LastCompletedDate.Date < DateTime.Today.AddDays(-1))
                    {
                        quest.CurrentStreak = 0;
                    }
                    await _context.SaveChangesAsync();
                }
            }

            ViewData["DailyQuest"] = quest;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CompleteQuest()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Index", "Front");
        }

        var user = await _userManager.GetUserAsync(User);
        var quest = await _context.DailyQuests
            .FirstOrDefaultAsync(d => d.UserId == user.Id && d.QuestName == "Make Your Bed");

        if (quest != null && !quest.CompletedToday)
        {
            var previousCompletionDate = quest.LastCompletedDate.Date;
            quest.CompletedToday = true;
            quest.LastCompletedDate = DateTime.Now;
    
            // If this is their first time completing the quest
            if (previousCompletionDate == DateTime.MinValue)
            {
                quest.CurrentStreak = 1;
            }
            // If they completed it yesterday, increment streak
            else if (previousCompletionDate == DateTime.Today.AddDays(-1))
            {
                quest.CurrentStreak++;
            }
            // If they missed a day or more, reset streak to 1
            else if (previousCompletionDate < DateTime.Today.AddDays(-1))
            {
                quest.CurrentStreak = 1;
            }
            // If they already completed it today (shouldn't happen due to CompletedToday check)
            else if (previousCompletionDate == DateTime.Today)
            {
                // Keep current streak
            }

            // Calculate and add XP
            int xpReward = quest.CalculateXPReward();
            bool leveledUp = user.AddXP(xpReward);

            await _context.SaveChangesAsync();

            // Return JSON result for AJAX update
            return Json(new { 
                success = true, 
                streak = quest.CurrentStreak, 
                xpGained = xpReward,
                leveledUp = leveledUp,
                newXP = user.CurrentXP,
                xpToNext = user.XPToNextLevel,
                currentLevel = user.CurrentLevel
            });
        }

        return Json(new { success = false });
    }
}