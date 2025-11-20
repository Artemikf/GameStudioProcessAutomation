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
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Password).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Role).IsRequired().HasMaxLength(20);

            modelBuilder.Entity<Employee>()
                .Property(e => e.Salary)
                .HasPrecision(18, 2); // 18 цифр всего, 2 после запятой

            modelBuilder.Entity<Project>()
                .Property(p => p.Budget)
                .HasPrecision(18, 2);

        });
    }

}

