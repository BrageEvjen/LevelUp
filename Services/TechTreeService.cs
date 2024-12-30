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

        // Gets the dataabse context when the service starts so we can work with the database. 
        public async Task UpdateNodeStatus(int nodeId)
        {
            // Get the node and its prerequisites from database
            var node = await _context.TechTreeNodes
                .Include(n => n.Prerequisites)  // Make sure to load prerequisites
                .FirstOrDefaultAsync(n => n.Id == nodeId);

            if (node == null) return;

            // Check if ALL prerequisites are completed
            bool allPrerequisitesComplete = node.Prerequisites
                .All(p => p.Status == "Complete");

            // If all prerequisites are done, make this node available
            if (allPrerequisitesComplete)
            {
                node.Status = "Available";
                await _context.SaveChangesAsync();
            }
        }

        // Call this when a node is completed
        public async Task CompleteNode(int nodeId)
        {
            var node = await _context.TechTreeNodes.FindAsync(nodeId);
            if (node == null) return;

            // Get current user
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            if (user == null) return;

            // Update node status
            node.Status = "Complete";
        
            // Award XP to user
            user.AddXP(node.XPReward);
        
            // Save both changes
            await _context.SaveChangesAsync();
            await _userManager.UpdateAsync(user);

            // Update dependent nodes
            var dependentNodes = await _context.TechTreeNodes
                .Where(n => n.Prerequisites.Any(p => p.Id == nodeId))
                .ToListAsync();

            foreach (var dependentNode in dependentNodes)
            {
                await UpdateNodeStatus(dependentNode.Id);
            }
        }
    }}