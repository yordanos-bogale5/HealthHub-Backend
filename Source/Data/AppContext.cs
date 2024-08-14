using HealthHub.Source.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthHub.Source.Data
{
  /// <summary>
  /// Represents the database context for the HealthHub application using Entity Framework Core.
  /// This class is responsible for configuring the database schema and managing entity objects.
  /// </summary>
  public class ApplicationContext : DbContext
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ApplicationContext"/> class.
    /// </summary>
    /// <param name="options">The options used to configure the <see cref="DbContext"/> instance.</param>
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

    /// <summary>
    /// Configures the model and its relationships using the provided <see cref="ModelBuilder"/>.
    /// </summary>
    /// <param name="modelBuilder">The builder used to configure the model's entity types and relationships.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      EntityConfiguration.ConfigureUniqueConstraints(modelBuilder);
    }

    /// <summary>
    /// Gets or sets the <see cref="DbSet{User}"/> for the <see cref="User"/> entity.
    /// This property represents the collection of <see cref="User"/> entities in the database.
    /// </summary>
    public DbSet<User> Users { get; set; }
  }
}


internal static class EntityConfiguration
{
  internal static void ConfigureUniqueConstraints(ModelBuilder mb)
  {
    // User Entity Configurations
    var user = mb.Entity<User>();

    user.HasIndex(u => u.Email).IsUnique();
    user.HasIndex(u => u.Phone).IsUnique();

  }
}