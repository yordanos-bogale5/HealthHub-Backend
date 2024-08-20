using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthHub.Migrations {
  /// <inheritdoc />
  public partial class RemovedSpecialityID_FromDoctorModel_And_AddedDoctorId_To_SpecialityModel : Migration {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropForeignKey(
          name: "FK_Doctors_Specialities_SpecialityId",
          table: "Doctors");

      migrationBuilder.DropIndex(
          name: "IX_Doctors_SpecialityId",
          table: "Doctors");

      migrationBuilder.DropColumn(
          name: "SpecialityId",
          table: "Doctors");

      migrationBuilder.AddColumn<Guid>(
          name: "DoctorId",
          table: "Specialities",
          type: "uniqueidentifier",
          nullable: false,
          defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

      migrationBuilder.CreateIndex(
          name: "IX_Specialities_DoctorId",
          table: "Specialities",
          column: "DoctorId");

      migrationBuilder.AddForeignKey(
          name: "FK_Specialities_Doctors_DoctorId",
          table: "Specialities",
          column: "DoctorId",
          principalTable: "Doctors",
          principalColumn: "DoctorId",
          onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropForeignKey(
          name: "FK_Specialities_Doctors_DoctorId",
          table: "Specialities");

      migrationBuilder.DropIndex(
          name: "IX_Specialities_DoctorId",
          table: "Specialities");

      migrationBuilder.DropColumn(
          name: "DoctorId",
          table: "Specialities");

      migrationBuilder.AddColumn<Guid>(
          name: "SpecialityId",
          table: "Doctors",
          type: "uniqueidentifier",
          nullable: false,
          defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

      migrationBuilder.CreateIndex(
          name: "IX_Doctors_SpecialityId",
          table: "Doctors",
          column: "SpecialityId");

      migrationBuilder.AddForeignKey(
          name: "FK_Doctors_Specialities_SpecialityId",
          table: "Doctors",
          column: "SpecialityId",
          principalTable: "Specialities",
          principalColumn: "SpecialityId",
          onDelete: ReferentialAction.Cascade);
    }
  }
}
