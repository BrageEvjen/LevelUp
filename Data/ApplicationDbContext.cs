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

        
        // Tech Tree 1: Running
        modelBuilder.Entity<TechTreeNode>().HasData(
            new TechTreeNode
            {
                Id = 1,
                TechTreeId = 1,
                Name = "Running Basics",
                Status = "Available",
                XPReward = 100,
                Description = "Learn proper running form and warm-up techniques",
                X = 1000,
                Y = 300
            },
            new TechTreeNode
            {
                Id = 2,
                TechTreeId = 1,
                Name = "5K Training",
                Status = "Locked",
                XPReward = 200,
                Description = "Start your journey to running a 5K",
                X = 1200,
                Y = 300
            },
            new TechTreeNode
            {
                Id = 3,
                TechTreeId = 1,
                Name = "10K Milestone",
                Status = "Locked",
                XPReward = 300,
                Description = "Prepare for your first 10K run",
                X = 1400,
                Y = 300
            }
        );

        // Tech Tree 2: Strength Training
        modelBuilder.Entity<TechTreeNode>().HasData(
            new TechTreeNode
            {
                Id = 4,
                TechTreeId = 2,
                Name = "Bodyweight Basics",
                Status = "Available",
                XPReward = 100,
                Description = "Master fundamental bodyweight exercises",
                X = 1000,
                Y = 300
            },
            new TechTreeNode
            {
                Id = 5,
                TechTreeId = 2,
                Name = "Dumbbell Training",
                Status = "Locked",
                XPReward = 200,
                Description = "Introduction to dumbbell exercises",
                X = 1200,
                Y = 300
            },
            new TechTreeNode
            {
                Id = 6,
                TechTreeId = 2,
                Name = "Barbell Mastery",
                Status = "Locked",
                XPReward = 300,
                Description = "Advanced barbell compound movements",
                X = 1400,
                Y = 300
            }
        );

        // Tech Tree 3: Flexibility
        modelBuilder.Entity<TechTreeNode>().HasData(
            new TechTreeNode
            {
                Id = 7,
                TechTreeId = 3,
                Name = "Basic Stretching",
                Status = "Available",
                XPReward = 100,
                Description = "Learn essential stretching techniques",
                X = 1000,
                Y = 300
            },
            new TechTreeNode
            {
                Id = 8,
                TechTreeId = 3,
                Name = "Yoga Foundations",
                Status = "Locked",
                XPReward = 200,
                Description = "Introduction to yoga poses and breathing",
                X = 1200,
                Y = 300
            },
            new TechTreeNode
            {
                Id = 9,
                TechTreeId = 3,
                Name = "Advanced Flexibility",
                Status = "Locked",
                XPReward = 300,
                Description = "Advanced stretching and mobility work",
                X = 1400,
                Y = 300
            }
        );

        // Set up prerequisites
        modelBuilder.Entity<TechTreeNode>()
            .HasMany(t => t.Prerequisites)
            .WithMany(t => t.UnlockedNodes)
            .UsingEntity(j => j.HasData(
                // Running Tree Prerequisites
                new { PrerequisitesId = 1, UnlockedNodesId = 2 },  // Running Basics -> 5K Training
                new { PrerequisitesId = 2, UnlockedNodesId = 3 },  // 5K Training -> 10K Milestone

                // Strength Training Prerequisites
                new { PrerequisitesId = 4, UnlockedNodesId = 5 },  // Bodyweight Basics -> Dumbbell Training
                new { PrerequisitesId = 5, UnlockedNodesId = 6 },  // Dumbbell Training -> Barbell Mastery

                // Flexibility Prerequisites
                new { PrerequisitesId = 7, UnlockedNodesId = 8 },  // Basic Stretching -> Yoga Foundations
                new { PrerequisitesId = 8, UnlockedNodesId = 9 }   // Yoga Foundations -> Advanced Flexibility
            ));
    }
}
