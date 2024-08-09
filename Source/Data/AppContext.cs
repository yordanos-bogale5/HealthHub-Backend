using HealthHub.Source.Models.Entities;
using Microsoft.EntityFrameworkCore;

public class AppContext : DbContext
{
  public AppContext(DbContextOptions options) : base(options) { }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    EntityConfiguration.ConfigureUniqueConstraints(modelBuilder);
  }

  public DbSet<User> Users { get; set; }

}

internal static class EntityConfiguration
{
  internal static void ConfigureUniqueConstraints(ModelBuilder mb)
  {
    /// User Entity Configurations
    var user = mb.Entity<User>();

    user.HasIndex(u => u.Email).IsUnique();
    user.HasIndex(u => u.Phone).IsUnique();

  }
}