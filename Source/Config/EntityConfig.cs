using HealthHub.Source.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace HealthHub.Source.Config;

public static class EntityConfiguration
{
  public static void ConfigureUniqueConstraints(ModelBuilder mb)
  {
    // User Entity Configurations
    var user = mb.Entity<User>();

    user.HasIndex(u => u.Email).IsUnique();
    user.HasIndex(u => u.Phone).IsUnique();

    var speciality = mb.Entity<Speciality>();
    speciality.HasIndex(s => s.SpecialityName).IsUnique();

    var blog = mb.Entity<Blog>();
    blog.HasIndex(b => b.Slug).IsUnique();

    var tag = mb.Entity<Tag>();
    tag.HasIndex(b => b.TagName).IsUnique();
  }

  public static void ConfigureForeignKeyConstraints(ModelBuilder mb)
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

    var message = mb.Entity<Message>();

    // Delete cascade all messages related when a Conversation is deleted
    message
      .HasOne(m => m.Conversation)
      .WithMany(c => c.Messages)
      .HasForeignKey(m => m.ConversationId)
      .OnDelete(DeleteBehavior.Cascade);

    message
      .HasOne(m => m.Sender)
      .WithMany()
      .HasForeignKey(m => m.SenderId)
      .OnDelete(DeleteBehavior.NoAction);

    message
      .HasOne(m => m.Receiver)
      .WithMany()
      .HasForeignKey(m => m.ReceiverId)
      .OnDelete(DeleteBehavior.NoAction);

    var payment = mb.Entity<Payment>();

    // Restrict the deletion of the Payment table if the Sender entity is deleted
    payment
      .HasOne(p => p.Sender)
      .WithMany()
      .HasForeignKey(p => p.SenderId)
      .OnDelete(DeleteBehavior.Restrict);

    // Restrict the deletion of the Payment table if the Receiver Entity is deleted
    payment
      .HasOne(p => p.Receiver)
      .WithMany()
      .HasForeignKey(p => p.ReceiverId)
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

    var blogTag = mb.Entity<BlogTag>();

    blogTag.HasKey(bt => new { bt.BlogId, bt.TagId });

    blogTag
      .HasOne(b => b.Blog)
      .WithMany(b => b.BlogTags)
      .HasForeignKey(bt => bt.BlogId)
      .OnDelete(DeleteBehavior.Cascade);

    blogTag
      .HasOne(b => b.Tag)
      .WithMany(b => b.BlogTags)
      .HasForeignKey(bt => bt.TagId)
      .OnDelete(DeleteBehavior.NoAction);

    // Doctor Availability
    mb.Entity<DoctorAvailability>()
      .HasOne(da => da.Doctor)
      .WithMany(d => d.DoctorAvailabilities)
      .HasForeignKey(da => da.DoctorId)
      .OnDelete(DeleteBehavior.Cascade);

    // Doctor Appointments
    mb.Entity<Appointment>()
      .HasOne(a => a.Doctor)
      .WithMany(d => d.Appointments)
      .HasForeignKey(a => a.DoctorId)
      .OnDelete(DeleteBehavior.Restrict);

    // Patient Appointments
    mb.Entity<Appointment>()
      .HasOne(a => a.Patient)
      .WithMany(p => p.Appointments)
      .HasForeignKey(a => a.PatientId)
      .OnDelete(DeleteBehavior.Cascade);

    // Configure Doctor education relationship
    mb.Entity<Doctor>()
      .HasMany(d => d.Educations)
      .WithOne(e => e.Doctor)
      .HasForeignKey(e => e.DoctorId)
      .OnDelete(DeleteBehavior.Cascade);

    // Configure doctor experience relationship
    mb.Entity<Doctor>()
      .HasMany(d => d.Experiences)
      .WithOne(e => e.Doctor)
      .HasForeignKey(e => e.DoctorId)
      .OnDelete(DeleteBehavior.Cascade);

    // Configure Cv relationship with doctor
    mb.Entity<Doctor>()
      .HasOne(d => d.Cv)
      .WithOne()
      .HasForeignKey<Doctor>(d => d.CvId)
      .OnDelete(DeleteBehavior.Cascade);

    var conversationMemberships = mb.Entity<ConversationMembership>();
    conversationMemberships.HasKey(cm => new { cm.UserId, cm.ConversationId });
  }

  public static void ConfigureEnumConversions(ModelBuilder mb)
  {
    // For User
    mb.Entity<User>().Property(u => u.Role).HasConversion<string>();
    mb.Entity<User>().Property(u => u.Gender).HasConversion<string>();

    // For Doctor
    mb.Entity<Doctor>().Property(d => d.DoctorStatus).HasConversion<string>();

    // For Appointment
    mb.Entity<Appointment>().Property(a => a.AppointmentType).HasConversion<string>();
    mb.Entity<Appointment>().Property(a => a.Status).HasConversion<string>();

    // For Paymentt
    mb.Entity<Payment>().Property(r => r.PaymentStatus).HasConversion<string>();
    mb.Entity<Payment>().Property(r => r.PaymentProvider).HasConversion<string>();

    // For Notification
    mb.Entity<Notification>().Property(r => r.NotificationType).HasConversion<string>();

    // For DoctorAvailability
    mb.Entity<DoctorAvailability>().Property(da => da.AvailableDay).HasConversion<string>();
  }
}
