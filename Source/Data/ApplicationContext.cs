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

    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      EntityConfiguration.ConfigureUniqueConstraints(modelBuilder);
      EntityConfiguration.ConfigureForeignKeys(modelBuilder);
    }


    public DbSet<User> Users { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Speciality> Specialities { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<DoctorAvailability> DoctorAvailabilities { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<BlogComment> BlogComments { get; set; }
    public DbSet<BlogLike> BlogLikes { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Chat> Chats { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Models.Entities.File> Files { get; set; }

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

  internal static void ConfigureForeignKeys(ModelBuilder mb)
  {
    var appointment = mb.Entity<Appointment>();

    // Restrict the deletion of the Appointment table if the Doctor entity is deleted
    appointment
    .HasOne(a => a.Doctor)
    .WithMany()
    .HasForeignKey(a => a.DoctorId)
    .OnDelete(DeleteBehavior.Restrict);

    // Restrict the deletion of the Appointment table if the Patient entity is deleted
    appointment
    .HasOne(a => a.Patient)
    .WithMany()
    .HasForeignKey(a => a.PatientId)
    .OnDelete(DeleteBehavior.Restrict);


    var chat = mb.Entity<Chat>();

    // Restrict the deletion of the Chat table if the Doctor entity is deleted
    chat
    .HasOne(c => c.Sender)
    .WithMany()
    .HasForeignKey(c => c.SenderId)
    .OnDelete(DeleteBehavior.Restrict);

    // Restrict the deletion of the Chat table if the Patient entity is deleted
    chat
    .HasOne(c => c.Receiver)
    .WithMany()
    .HasForeignKey(c => c.ReceiverId)
    .OnDelete(DeleteBehavior.Restrict);


    var payment = mb.Entity<Payment>();

    // Restrict the deletion of the Payment table if the Doctor entity is deleted
    payment
    .HasOne(p => p.Doctor)
    .WithMany()
    .HasForeignKey(p => p.DoctorId)
    .OnDelete(DeleteBehavior.Restrict);

    // Restrict the deletion of the Payment table if the Patient entity is deleted
    payment
    .HasOne(p => p.Patient)
    .WithMany()
    .HasForeignKey(p => p.PatientId)
    .OnDelete(DeleteBehavior.Restrict);

  }
}