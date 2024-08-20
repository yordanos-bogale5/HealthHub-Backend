using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthHub.Migrations {
  /// <inheritdoc />
  public partial class Added_Patient_Doctor_Table : Migration {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) {
      migrationBuilder.CreateTable(
          name: "Patients",
          columns: table => new {
            PatientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            MedicalHistory = table.Column<string>(type: "nvarchar(max)", nullable: true),
            EmergencyContactName = table.Column<string>(type: "nvarchar(max)", nullable: true),
            EmergencyContactPhone = table.Column<string>(type: "nvarchar(max)", nullable: true)
          },
          constraints: table => {
            table.PrimaryKey("PK_Patients", x => x.PatientId);
            table.ForeignKey(
                      name: "FK_Patients_Users_UserId",
                      column: x => x.UserId,
                      principalTable: "Users",
                      principalColumn: "UserId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateTable(
          name: "Speciality",
          columns: table => new {
            SpecialityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            SpecialityName = table.Column<string>(type: "nvarchar(max)", nullable: false),
            CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_Speciality", x => x.SpecialityId);
          });

      migrationBuilder.CreateTable(
          name: "Doctors",
          columns: table => new {
            DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            SpecialityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            Qualifications = table.Column<string>(type: "nvarchar(max)", nullable: false),
            Biography = table.Column<string>(type: "nvarchar(max)", nullable: false),
            DoctorStatus = table.Column<int>(type: "int", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_Doctors", x => x.DoctorId);
            table.ForeignKey(
                      name: "FK_Doctors_Speciality_SpecialityId",
                      column: x => x.SpecialityId,
                      principalTable: "Speciality",
                      principalColumn: "SpecialityId",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_Doctors_Users_UserId",
                      column: x => x.UserId,
                      principalTable: "Users",
                      principalColumn: "UserId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_Doctors_SpecialityId",
          table: "Doctors",
          column: "SpecialityId");

      migrationBuilder.CreateIndex(
          name: "IX_Doctors_UserId",
          table: "Doctors",
          column: "UserId");

      migrationBuilder.CreateIndex(
          name: "IX_Patients_UserId",
          table: "Patients",
          column: "UserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropTable(
          name: "Doctors");

      migrationBuilder.DropTable(
          name: "Patients");

      migrationBuilder.DropTable(
          name: "Speciality");
    }
  }
}
