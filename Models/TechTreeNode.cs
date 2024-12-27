using Microsoft.AspNetCore.Identity;

namespace LevelUp.Models
{
    public class TechTreeNode
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public List<string> Prerequisites { get; set; }
        public int XPReward { get; set; }
        public string Description { get; set; }
    }
}
