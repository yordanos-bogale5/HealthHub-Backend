using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthHub.Migrations {
  /// <inheritdoc />
  public partial class Added_Remaining_Db_Sets : Migration {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropForeignKey(
          name: "FK_Doctors_Speciality_SpecialityId",
          table: "Doctors");

      migrationBuilder.DropPrimaryKey(
          name: "PK_Speciality",
          table: "Speciality");

      migrationBuilder.RenameTable(
          name: "Speciality",
          newName: "Specialities");

      migrationBuilder.AddPrimaryKey(
          name: "PK_Specialities",
          table: "Specialities",
          column: "SpecialityId");

      migrationBuilder.CreateTable(
          name: "Admins",
          columns: table => new {
            AdminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_Admins", x => x.AdminId);
            table.ForeignKey(
                      name: "FK_Admins_Users_UserId",
                      column: x => x.UserId,
                      principalTable: "Users",
                      principalColumn: "UserId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "Appointments",
          columns: table => new {
            AppointmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            AppointmentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            AppointmentTime = table.Column<TimeOnly>(type: "time", nullable: false),
            AppointmentType = table.Column<int>(type: "int", nullable: false),
            Status = table.Column<int>(type: "int", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_Appointments", x => x.AppointmentId);
            table.ForeignKey(
                      name: "FK_Appointments_Doctors_DoctorId",
                      column: x => x.DoctorId,
                      principalTable: "Doctors",
                      principalColumn: "DoctorId",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "FK_Appointments_Patients_PatientId",
                      column: x => x.PatientId,
                      principalTable: "Patients",
                      principalColumn: "PatientId",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "Blogs",
          columns: table => new {
            BlogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_Blogs", x => x.BlogId);
            table.ForeignKey(
                      name: "FK_Blogs_Users_UserId",
                      column: x => x.UserId,
                      principalTable: "Users",
                      principalColumn: "UserId");
          });

      migrationBuilder.CreateTable(
          name: "Chats",
          columns: table => new {
            ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ReceiverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_Chats", x => x.ChatId);
            table.ForeignKey(
                      name: "FK_Chats_Users_ReceiverId",
                      column: x => x.ReceiverId,
                      principalTable: "Users",
                      principalColumn: "UserId",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "FK_Chats_Users_SenderId",
                      column: x => x.SenderId,
                      principalTable: "Users",
                      principalColumn: "UserId",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "DoctorAvailabilities",
          columns: table => new {
            DoctorAvailabilityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            AvailableDay = table.Column<int>(type: "int", nullable: false),
            StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
            EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_DoctorAvailabilities", x => x.DoctorAvailabilityId);
            table.ForeignKey(
                      name: "FK_DoctorAvailabilities_Doctors_DoctorId",
                      column: x => x.DoctorId,
                      principalTable: "Doctors",
                      principalColumn: "DoctorId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "File",
          columns: table => new {
            FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            MimeType = table.Column<string>(type: "nvarchar(max)", nullable: false),
            FileData = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_File", x => x.FileId);
          });

      migrationBuilder.CreateTable(
          name: "Notifications",
          columns: table => new {
            NotificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            NotificationType = table.Column<int>(type: "int", nullable: false),
            Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_Notifications", x => x.NotificationId);
            table.ForeignKey(
                      name: "FK_Notifications_Users_UserId",
                      column: x => x.UserId,
                      principalTable: "Users",
                      principalColumn: "UserId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "Payment",
          columns: table => new {
            PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            PaymentStatus = table.Column<int>(type: "int", nullable: false),
            PaymentProvider = table.Column<int>(type: "int", nullable: false),
            PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_Payment", x => x.PaymentId);
            table.ForeignKey(
                      name: "FK_Payment_Doctors_DoctorId",
                      column: x => x.DoctorId,
                      principalTable: "Doctors",
                      principalColumn: "DoctorId",
                      onDelete: ReferentialAction.Restrict);
            table.ForeignKey(
                      name: "FK_Payment_Patients_PatientId",
                      column: x => x.PatientId,
                      principalTable: "Patients",
                      principalColumn: "PatientId",
                      onDelete: ReferentialAction.Restrict);
          });

      migrationBuilder.CreateTable(
          name: "BlogComments",
          columns: table => new {
            BlogCommentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            BlogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            CommentText = table.Column<string>(type: "nvarchar(max)", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_BlogComments", x => x.BlogCommentId);
            table.ForeignKey(
                      name: "FK_BlogComments_Blogs_BlogId",
                      column: x => x.BlogId,
                      principalTable: "Blogs",
                      principalColumn: "BlogId",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_BlogComments_Users_SenderId",
                      column: x => x.SenderId,
                      principalTable: "Users",
                      principalColumn: "UserId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "BlogLikes",
          columns: table => new {
            BlogLikeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            BlogId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_BlogLikes", x => x.BlogLikeId);
            table.ForeignKey(
                      name: "FK_BlogLikes_Blogs_BlogId",
                      column: x => x.BlogId,
                      principalTable: "Blogs",
                      principalColumn: "BlogId",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_BlogLikes_Users_UserId",
                      column: x => x.UserId,
                      principalTable: "Users",
                      principalColumn: "UserId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "Messages",
          columns: table => new {
            MessageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            ChatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            MessageText = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table => {
            table.PrimaryKey("PK_Messages", x => x.MessageId);
            table.ForeignKey(
                      name: "FK_Messages_Chats_ChatId",
                      column: x => x.ChatId,
                      principalTable: "Chats",
                      principalColumn: "ChatId",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_Messages_File_FileId",
                      column: x => x.FileId,
                      principalTable: "File",
                      principalColumn: "FileId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "Reviews",
          columns: table => new {
            ReviewId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            PaymentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            StarRating = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
            ReviewText = table.Column<string>(type: "nvarchar(max)", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_Reviews", x => x.ReviewId);
            table.ForeignKey(
                      name: "FK_Reviews_Payment_PaymentId",
                      column: x => x.PaymentId,
                      principalTable: "Payment",
                      principalColumn: "PaymentId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_Admins_UserId",
          table: "Admins",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_Appointments_DoctorId",
          table: "Appointments",
          column: "DoctorId");

      migrationBuilder.CreateIndex(
          name: "IX_Appointments_PatientId",
          table: "Appointments",
          column: "PatientId");

      migrationBuilder.CreateIndex(
          name: "IX_BlogComments_BlogId",
          table: "BlogComments",
          column: "BlogId");

      migrationBuilder.CreateIndex(
          name: "IX_BlogComments_SenderId",
          table: "BlogComments",
          column: "SenderId");

      migrationBuilder.CreateIndex(
          name: "IX_BlogLikes_BlogId",
          table: "BlogLikes",
          column: "BlogId");

      migrationBuilder.CreateIndex(
          name: "IX_BlogLikes_UserId",
          table: "BlogLikes",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_Blogs_UserId",
          table: "Blogs",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_Chats_ReceiverId",
          table: "Chats",
          column: "ReceiverId");

      migrationBuilder.CreateIndex(
          name: "IX_Chats_SenderId",
          table: "Chats",
          column: "SenderId");

      migrationBuilder.CreateIndex(
          name: "IX_DoctorAvailabilities_DoctorId",
          table: "DoctorAvailabilities",
          column: "DoctorId");

      migrationBuilder.CreateIndex(
          name: "IX_Messages_ChatId",
          table: "Messages",
          column: "ChatId");

      migrationBuilder.CreateIndex(
          name: "IX_Messages_FileId",
          table: "Messages",
          column: "FileId");

      migrationBuilder.CreateIndex(
          name: "IX_Notifications_UserId",
          table: "Notifications",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_Payment_DoctorId",
          table: "Payment",
          column: "DoctorId");

      migrationBuilder.CreateIndex(
          name: "IX_Payment_PatientId",
          table: "Payment",
          column: "PatientId");

      migrationBuilder.CreateIndex(
          name: "IX_Reviews_PaymentId",
          table: "Reviews",
          column: "PaymentId");

      migrationBuilder.AddForeignKey(
          name: "FK_Doctors_Specialities_SpecialityId",
          table: "Doctors",
          column: "SpecialityId",
          principalTable: "Specialities",
          principalColumn: "SpecialityId",
          onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropForeignKey(
          name: "FK_Doctors_Specialities_SpecialityId",
          table: "Doctors");

      migrationBuilder.DropTable(
          name: "Admins");

      migrationBuilder.DropTable(
          name: "Appointments");

      migrationBuilder.DropTable(
          name: "BlogComments");

      migrationBuilder.DropTable(
          name: "BlogLikes");

      migrationBuilder.DropTable(
          name: "DoctorAvailabilities");

      migrationBuilder.DropTable(
          name: "Messages");

      migrationBuilder.DropTable(
          name: "Notifications");

      migrationBuilder.DropTable(
          name: "Reviews");

      migrationBuilder.DropTable(
          name: "Blogs");

      migrationBuilder.DropTable(
          name: "Chats");

      migrationBuilder.DropTable(
          name: "File");

      migrationBuilder.DropTable(
          name: "Payment");

      migrationBuilder.DropPrimaryKey(
          name: "PK_Specialities",
          table: "Specialities");

      migrationBuilder.RenameTable(
          name: "Specialities",
          newName: "Speciality");

      migrationBuilder.AddPrimaryKey(
          name: "PK_Speciality",
          table: "Speciality",
          column: "SpecialityId");

      migrationBuilder.AddForeignKey(
          name: "FK_Doctors_Speciality_SpecialityId",
          table: "Doctors",
          column: "SpecialityId",
          principalTable: "Speciality",
          principalColumn: "SpecialityId",
          onDelete: ReferentialAction.Cascade);
    }
  }
}
