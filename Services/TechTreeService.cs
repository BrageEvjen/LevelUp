using LevelUp.Data;
using LevelUp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LevelUp.Services
{
    // This is just the constructor
public class TechTreeService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TechTreeService(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> CompleteNode(int nodeId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var node = await _context.TechTreeNodes
                .FirstOrDefaultAsync(n => n.Id == nodeId && n.Status == "Available");
                
            if (node == null) return false;

            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user == null) return false;

            node.Status = "Complete";
            user.AddXP(node.XPReward);

            await _context.SaveChangesAsync();
            
            var dependentNodes = await _context.TechTreeNodes
                .Include(n => n.Prerequisites)
                .Where(n => n.Prerequisites.Any(p => p.Id == nodeId))
                .ToListAsync();

            foreach (var dependentNode in dependentNodes)
            {
                await UpdateNodeStatus(dependentNode.Id);
            }

            await _userManager.UpdateAsync(user);
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task UpdateNodeStatus(int nodeId)
    {
        var node = await _context.TechTreeNodes
            .Include(n => n.Prerequisites)
            .FirstOrDefaultAsync(n => n.Id == nodeId);

        if (node == null) return;

        if (node.Prerequisites.All(p => p.Status == "Complete"))
        {
            node.Status = "Available";
            await _context.SaveChangesAsync();
        }
    }
    }}