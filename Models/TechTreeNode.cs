using Microsoft.AspNetCore.Identity;

namespace LevelUp.Models
{
    public class TechTreeNode
    {
        public int Id { get; set; }
        public int TechTreeId { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public List<TechTreeNode> Prerequisites { get; set; }
        public List<TechTreeNode> UnlockedNodes { get; set; }
        public int XPReward { get; set; }
        public string Description { get; set; }
        public int X { get; set; }  // Add this
        public int Y { get; set; }  // Add this
    }
}
