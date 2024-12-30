using Microsoft.AspNetCore.Identity;

namespace LevelUp.Models
{
    public class TechTreeNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public List<TechTreeNode> Prerequisites { get; set; }
        public List<TechTreeNode> UnlockedNodes { get; set; }
        public int XPReward { get; set; }
        public string Description { get; set; }
    }
}
