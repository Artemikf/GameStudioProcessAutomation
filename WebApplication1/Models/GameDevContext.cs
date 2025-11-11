using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models;

public class GameDevContext : DbContext
{
    public GameDevContext(DbContextOptions<GameDevContext> options) : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<GameDesigner> GameDesigners { get; set; }
    public DbSet<Developer> Developers { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Tester> Testers { get; set; }
    public DbSet<Producer> Producers { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Task> Tasks { get; set; }
    public DbSet<GameConcept> GameConcepts { get; set; }
    public DbSet<TechnicalRequirements> TechnicalRequirements { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<BugReport> BugReports { get; set; }
}

