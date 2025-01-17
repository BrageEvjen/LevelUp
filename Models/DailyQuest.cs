using System;

namespace LevelUp.Models
{
    public class DailyQuest
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string QuestName { get; set; }
        public int CurrentStreak { get; set; }
        public DateTime LastCompletedDate { get; set; }
        public bool CompletedToday { get; set; }

        public int CalculateXPReward()
        {
            return 100 + (CurrentStreak * 10);
        }
        
    }
}