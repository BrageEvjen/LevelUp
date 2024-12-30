using Microsoft.AspNetCore.Identity;

namespace LevelUp.Models
{
    // Makes a model that inherits from IdentityUser and expands upon it
    public class ApplicationUser : IdentityUser
    {
        public int CurrentXP  { get; set; }
        public int CurrentLevel  { get; set; }
        public int XPToNextLevel  { get; set; }
        
        // Track the completed and uncompleted TechTree Nodes
        public List<string> CompletedNodes { get; set; } = new List<string>();
        public List<string> UnlockedNodes { get; set; } = new List<string>();
        
        // Constructor to set the initial values
        public ApplicationUser()
        {
            CurrentXP = 0;
            CurrentLevel = 1;
            XPToNextLevel = CalculateXPToNextLevel();
        }

        // Makes a method that calualted the XP it takes to reach the next level. 
        public int CalculateXPToNextLevel()
        {
            int baseXP = 100;
            double multiplier = 1.5;
            
            return (int) (baseXP * Math.Pow(CurrentLevel, multiplier));
        }

        // Makes a method that adds XP to the user and checks if the user leveled up. 
        public bool AddXP(int xpEarned)
        {
            CurrentXP += xpEarned;
            bool leveldUp = false;
            
            // Check if user has enough XP to level up
            while (CurrentXP >= XPToNextLevel)
            {
                CurrentXP -= XPToNextLevel;
                CurrentLevel++;
                leveldUp = true;
                
               // Recalculate XP required for next level 
               XPToNextLevel = CalculateXPToNextLevel();
            }
            return leveldUp;
        }
    }
}