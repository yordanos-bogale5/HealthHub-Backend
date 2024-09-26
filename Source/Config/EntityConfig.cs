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

    var payment = mb.Entity<Payment>();
    payment.HasIndex(p => p.TransactionReference).IsUnique();

    mb.Entity<DoctorSpeciality>().HasKey(ds => new { ds.DoctorId, ds.SpecialityId });

    mb.Entity<BlogTag>().HasKey(bt => new { bt.BlogId, bt.TagId });

    var conversationMemberships = mb.Entity<ConversationMembership>();
    conversationMemberships.HasKey(cm => new { cm.UserId, cm.ConversationId });

    var review = mb.Entity<Review>();
    review.HasKey(r => new { r.DoctorId, r.PatientId });

    var doctorPreference = mb.Entity<DoctorPreference>();
    doctorPreference.HasIndex(dp => dp.DoctorId).IsUnique();
  }

  public static void ConfigureForeignKeyConstraints(ModelBuilder mb)
  {
    var appointment = mb.Entity<Appointment>();

    appointment
      .HasOne(a => a.Doctor)
      .WithMany()
      .HasForeignKey(a => a.DoctorId)
      .OnDelete(DeleteBehavior.NoAction);

    appointment
      .HasOne(a => a.Patient)
      .WithMany()
      .HasForeignKey(a => a.PatientId)
      .OnDelete(DeleteBehavior.Cascade);

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

    // message
    //   .HasOne(m => m.Receiver)
    //   .WithMany()
    //   .HasForeignKey(m => m.ReceiverId)
    //   .OnDelete(DeleteBehavior.SetNull);

    var payment = mb.Entity<Payment>();

    // We don't want cascade property on payment record since it holds
    // Sensitive data that later might be deemed important
    payment
      .HasOne(p => p.Sender)
      .WithMany()
      .HasForeignKey(p => p.SenderId)
      .OnDelete(DeleteBehavior.Cascade);

    payment
      .HasOne(p => p.Receiver)
      .WithMany()
      .HasForeignKey(p => p.ReceiverId)
      .OnDelete(DeleteBehavior.NoAction);

    var doctorSpeciality = mb.Entity<DoctorSpeciality>();
    doctorSpeciality
      .HasOne(ds => ds.Doctor)
      .WithMany(d => d.DoctorSpecialities)
      .HasForeignKey(ds => ds.DoctorId)
      .OnDelete(DeleteBehavior.Cascade);

    doctorSpeciality
      .HasOne(ds => ds.Speciality)
      .WithMany(s => s.DoctorSpecialities)
      .HasForeignKey(ds => ds.SpecialityId)
      .OnDelete(DeleteBehavior.Cascade);

    var blog = mb.Entity<Blog>();
    blog.HasOne(b => b.Author)
      .WithMany(a => a.Blogs)
      .HasForeignKey(b => b.AuthorId)
      .OnDelete(DeleteBehavior.Cascade); // Cascade delete because a blog without an author reference will be a threat to accountability

    var blogComment = mb.Entity<BlogComment>();
    blogComment
      .HasOne(bc => bc.Blog)
      .WithMany(b => b.BlogComments)
      .HasForeignKey(bc => bc.BlogId)
      .OnDelete(DeleteBehavior.Cascade); // No need to keep a comment if a blog gets deleted

    blogComment
      .HasOne(bc => bc.Sender)
      .WithMany(s => s.BlogComments)
      .HasForeignKey(bc => bc.SenderId) // Corrected Foreign Key
      .OnDelete(DeleteBehavior.NoAction);

    var blogLike = mb.Entity<BlogLike>();
    blogLike
      .HasOne(bl => bl.Blog)
      .WithMany(b => b.BlogLikes)
      .HasForeignKey(bl => bl.BlogId)
      .OnDelete(DeleteBehavior.Cascade);

    blogLike
      .HasOne(bl => bl.User)
      .WithMany(u => u.BlogLikes)
      .HasForeignKey(bl => bl.UserId)
      .OnDelete(DeleteBehavior.NoAction);

    var blogTag = mb.Entity<BlogTag>();

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
      .OnDelete(DeleteBehavior.NoAction);

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

    var patient = mb.Entity<Patient>();
    var doctor = mb.Entity<Doctor>();

    // Configure patient review relationships
    patient
      .HasMany(p => p.Reviews)
      .WithOne(r => r.Patient)
      .HasForeignKey(r => r.PatientId)
      .OnDelete(DeleteBehavior.NoAction);

    // Configure doctor review relationships
    doctor
      .HasMany(d => d.Reviews)
      .WithOne(r => r.Doctor)
      .HasForeignKey(r => r.DoctorId)
      .OnDelete(DeleteBehavior.Cascade);

    doctor
      .HasOne(d => d.DoctorPreference)
      .WithOne(dp => dp.Doctor)
      .HasForeignKey<DoctorPreference>(dp => dp.DoctorId)
      .OnDelete(DeleteBehavior.Cascade);

    var conversation = mb.Entity<Conversation>();

    var conversationMembership = mb.Entity<ConversationMembership>();
    conversationMembership
      .HasOne(cm => cm.Conversation)
      .WithMany(c => c.ConversationMemberships)
      .HasForeignKey(cm => cm.ConversationId)
      .OnDelete(DeleteBehavior.Cascade);

    conversationMembership
      .HasOne(cm => cm.User)
      .WithMany(u => u.ConversationMemberships)
      .HasForeignKey(cm => cm.UserId)
      .OnDelete(DeleteBehavior.NoAction);
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
