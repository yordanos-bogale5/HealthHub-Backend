using HealthHub.Source.Config;
using HealthHub.Source.Helpers.Defaults;
using HealthHub.Source.Models.Entities;
using HealthHub.Source.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Org.BouncyCastle.Crypto.Signers;

namespace HealthHub.Source.Data;

/// <summary>
/// Represents the database context for the HealthHub application using Entity Framework Core.
/// This class is responsible for configuring the database schema and managing entity objects.
/// </summary>
public class ApplicationContext : DbContext
{
  public DbSet<User> Users { get; set; }
  public DbSet<Doctor> Doctors { get; set; }
  public DbSet<DoctorPreference> DoctorPreferences { get; set; }
  public DbSet<Patient> Patients { get; set; }
  public DbSet<Admin> Admins { get; set; }
  public DbSet<Speciality> Specialities { get; set; }
  public DbSet<DoctorSpeciality> DoctorSpecialities { get; set; }

  public DbSet<Education> Educations { get; set; }
  public DbSet<Experience> Experiences { get; set; }

  public DbSet<Appointment> Appointments { get; set; }
  public DbSet<DoctorAvailability> DoctorAvailabilities { get; set; }
  public DbSet<Blog> Blogs { get; set; }
  public DbSet<BlogComment> BlogComments { get; set; }
  public DbSet<BlogLike> BlogLikes { get; set; }
  public DbSet<Tag> Tags { get; set; }
  public DbSet<BlogTag> BlogTags { get; set; }

  public DbSet<Review> Reviews { get; set; }

  public DbSet<Conversation> Conversations { get; set; }
  public DbSet<ConversationMembership> ConversationMemberships { get; set; }
  public DbSet<Message> Messages { get; set; }

  public DbSet<Notification> Notifications { get; set; }

  public DbSet<Models.Entities.File> Files { get; set; }
  public DbSet<FileAssociation> FileAssociations { get; set; }

  public DbSet<Payment> Payments { get; set; }

  public ApplicationContext(DbContextOptions<ApplicationContext> options)
    : base(options) { }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    EntityConfiguration.ConfigureUniqueConstraints(modelBuilder);
    EntityConfiguration.ConfigureForeignKeyConstraints(modelBuilder);
    EntityConfiguration.ConfigureEnumConversions(modelBuilder);

    modelBuilder
      .Entity<FileAssociation>()
      .HasDiscriminator<DiscriminatorTypes>("EntityType")
      .HasValue<MessageFileAssociation>(DiscriminatorTypes.Message);
    // Add more discriminators here if needed
  }

  public override int SaveChanges()
  {
    var entries = ChangeTracker
      .Entries()
      .Where(e => e.Entity is BaseEntity && e.State == EntityState.Modified);

    // Each time an entity of type BaseEntity is modified we update the field UpdatedAt to reflect the correct time
    foreach (var entry in entries)
    {
      ((BaseEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;
    }

    return base.SaveChanges();
  }

  public override Task<int> SaveChangesAsync(
    bool acceptAllChangesOnSuccess,
    CancellationToken cancellationToken = default
  )
  {
    var entries = ChangeTracker
      .Entries()
      .Where(e => e.Entity is BaseEntity && e.State == EntityState.Modified);

    // Each time an entity of type BaseEntity is modified we update the field UpdatedAt to reflect the correct time
    foreach (var entry in entries)
    {
      ((BaseEntity)entry.Entity).UpdatedAt = DateTime.UtcNow;
    }

    return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
  }
}
