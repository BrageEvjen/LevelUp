using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using LevelUp.Models;
using Microsoft.AspNetCore.Identity;

namespace LevelUp.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    // Add DbSet for TechTreeNode
    public DbSet<TechTreeNode> TechTreeNodes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Makes it so a node can be connected to other nodes. Makes that possibility of needing to unclock other nodes first
        modelBuilder.Entity<TechTreeNode>()
            .HasMany(t => t.Prerequisites)
            .WithMany(t => t.UnlockedNodes)
            .UsingEntity(j => j.ToTable("TechTreePrerequisites"));
        
        // Create a password hasher
        var hasher = new PasswordHasher<ApplicationUser>();
    
        // Create the default user
        var defaultUser = new ApplicationUser
        {
            Id = "1", // Using a string as Id is typical for IdentityUser
            UserName = "test@example.com",
            NormalizedUserName = "TEST@EXAMPLE.COM",
            Email = "test@example.com",
            NormalizedEmail = "TEST@EXAMPLE.COM",
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString(),
            ConcurrencyStamp = Guid.NewGuid().ToString(),
        };
    
        // Set the password hash
        defaultUser.PasswordHash = hasher.HashPassword(defaultUser, "Test123!");

        // Seed the user
        modelBuilder.Entity<ApplicationUser>().HasData(defaultUser);

        
        // Insert TechTreeNodes here!
        modelBuilder.Entity<TechTreeNode>().HasData(
            new TechTreeNode
            {
                Id = 1, 
                Name = "Run 1Km",
                Status = "Available",
                XPReward = 100,
                Description = "Start your running journey with a solid and impressive goal! Running a total of 1000 meters"
            },
            new TechTreeNode
            {
                Id = 2,
                Name = "Run 2km",
                Status = "Locked",
                XPReward = 200,
                Description = "Complete this impressive milestone of DOUBLING your previous goal! By running a total of 2000 meters!!"
            },
            new TechTreeNode
            {
                Id = 3,
                Name = "Run 5km",
                Status = "Locked",
                XPReward = 400,
                Description = "This is a big goal. This is a real runners milestone. Being a human that is able to run 5000 meters is really impressive!"
            }
        );
        
        modelBuilder.Entity<TechTreeNode>()
            .HasMany(t => t.Prerequisites)
            .WithMany(t => t.UnlockedNodes)
            .UsingEntity(j => j.HasData(
                new { PrerequisitesId = 1, UnlockedNodesId = 2 },  // Run 1km unlocks Run 2km
                new { PrerequisitesId = 2, UnlockedNodesId = 3 }   // Run 2km unlocks Run 5km
            ));
    }
}
