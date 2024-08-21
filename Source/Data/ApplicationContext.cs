using HealthHub.Source.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Signers;

namespace HealthHub.Source.Data
{
  /// <summary>
  /// Represents the database context for the HealthHub application using Entity Framework Core.
  /// This class is responsible for configuring the database schema and managing entity objects.
  /// </summary>
  public class ApplicationContext : DbContext
  {
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
      : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      EntityConfiguration.ConfigureUniqueConstraints(modelBuilder);
      EntityConfiguration.ConfigureForeignKeys(modelBuilder);
      EntityConfiguration.ConfigureEnumConversions(modelBuilder);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<Speciality> Specialities { get; set; }
    public DbSet<DoctorSpeciality> DoctorSpecialities { get; set; }
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

    var speciality = mb.Entity<Speciality>();
    speciality.HasIndex(s => s.SpecialityName).IsUnique();
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
    chat.HasOne(c => c.Sender)
      .WithMany()
      .HasForeignKey(c => c.SenderId)
      .OnDelete(DeleteBehavior.Restrict);

    // Restrict the deletion of the Chat table if the Patient entity is deleted
    chat.HasOne(c => c.Receiver)
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

    mb.Entity<DoctorSpeciality>().HasKey(ds => new { ds.DoctorId, ds.SpecialityId }); // Comp Key

    mb.Entity<DoctorSpeciality>()
      .HasOne(ds => ds.Doctor)
      .WithMany(d => d.DoctorSpecialities)
      .HasForeignKey(ds => ds.DoctorId)
      .OnDelete(DeleteBehavior.Cascade);

    mb.Entity<DoctorSpeciality>()
      .HasOne(ds => ds.Speciality)
      .WithMany(s => s.DoctorSpecialities)
      .HasForeignKey(ds => ds.SpecialityId)
      .OnDelete(DeleteBehavior.Cascade);

    // This can be optional if we want to preserve the blog even if the user gets deleted
    // or Cascade deletion of blog when the user who wrote that blog gets deleted
    mb.Entity<Blog>()
      .HasOne(b => b.Author)
      .WithMany(a => a.Blogs)
      .HasForeignKey(b => b.AuthorId)
      .OnDelete(DeleteBehavior.NoAction);

    // Same here too
    // Cascade delete Blog Comment when Blog or User who wrote that comment gets deleted
    mb.Entity<BlogComment>()
      .HasOne(bc => bc.Blog)
      .WithMany(b => b.BlogComments)
      .HasForeignKey(bc => bc.BlogId)
      .OnDelete(DeleteBehavior.Cascade); // No need to keep a comment if a blog gets deleted

    mb.Entity<BlogComment>()
      .HasOne(bc => bc.Sender)
      .WithMany(s => s.BlogComments)
      .HasForeignKey(bc => bc.SenderId) // Corrected Foreign Key
      .OnDelete(DeleteBehavior.Restrict); // Restrict delete to keep comments from deleted users

    // Doctor Availability
    mb.Entity<DoctorAvailability>()
      .HasOne(da => da.Doctor)
      .WithMany(d => d.DoctorAvailabilities)
      .HasForeignKey(da => da.DoctorId)
      .OnDelete(DeleteBehavior.Cascade);
  }

  internal static void ConfigureEnumConversions(ModelBuilder mb)
  {
    // For User
    mb.Entity<User>().Property(u => u.Role).HasConversion<string>();
    mb.Entity<User>().Property(u => u.Gender).HasConversion<string>();

    // For Doctor
    mb.Entity<Doctor>().Property(d => d.DoctorStatus).HasConversion<string>();

    // For Appointment
    mb.Entity<Appointment>().Property(a => a.AppointmentType).HasConversion<string>();
    mb.Entity<Appointment>().Property(a => a.Status).HasConversion<string>();

    // For Payment
    mb.Entity<Payment>().Property(r => r.PaymentStatus).HasConversion<string>();
    mb.Entity<Payment>().Property(r => r.PaymentProvider).HasConversion<string>();

    // For Notification
    mb.Entity<Notification>().Property(r => r.NotificationType).HasConversion<string>();

    // For DoctorAvailability
    mb.Entity<DoctorAvailability>().Property(da => da.AvailableDay).HasConversion<string>();
  }
}
