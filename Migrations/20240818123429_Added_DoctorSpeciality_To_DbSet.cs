using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthHub.Migrations {
  /// <inheritdoc />
  public partial class Added_DoctorSpeciality_To_DbSet : Migration {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropForeignKey(
          name: "FK_DoctorSpeciality_Doctors_DoctorId",
          table: "DoctorSpeciality");

      migrationBuilder.DropForeignKey(
          name: "FK_DoctorSpeciality_Specialities_SpecialityId",
          table: "DoctorSpeciality");

      migrationBuilder.DropPrimaryKey(
          name: "PK_DoctorSpeciality",
          table: "DoctorSpeciality");

      migrationBuilder.RenameTable(
          name: "DoctorSpeciality",
          newName: "DoctorSpecialities");

      migrationBuilder.RenameIndex(
          name: "IX_DoctorSpeciality_SpecialityId",
          table: "DoctorSpecialities",
          newName: "IX_DoctorSpecialities_SpecialityId");

      migrationBuilder.AddPrimaryKey(
          name: "PK_DoctorSpecialities",
          table: "DoctorSpecialities",
          columns: new[] { "DoctorId", "SpecialityId" });

      migrationBuilder.AddForeignKey(
          name: "FK_DoctorSpecialities_Doctors_DoctorId",
          table: "DoctorSpecialities",
          column: "DoctorId",
          principalTable: "Doctors",
          principalColumn: "DoctorId",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "FK_DoctorSpecialities_Specialities_SpecialityId",
          table: "DoctorSpecialities",
          column: "SpecialityId",
          principalTable: "Specialities",
          principalColumn: "SpecialityId",
          onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropForeignKey(
          name: "FK_DoctorSpecialities_Doctors_DoctorId",
          table: "DoctorSpecialities");

      migrationBuilder.DropForeignKey(
          name: "FK_DoctorSpecialities_Specialities_SpecialityId",
          table: "DoctorSpecialities");

      migrationBuilder.DropPrimaryKey(
          name: "PK_DoctorSpecialities",
          table: "DoctorSpecialities");

      migrationBuilder.RenameTable(
          name: "DoctorSpecialities",
          newName: "DoctorSpeciality");

      migrationBuilder.RenameIndex(
          name: "IX_DoctorSpecialities_SpecialityId",
          table: "DoctorSpeciality",
          newName: "IX_DoctorSpeciality_SpecialityId");

      migrationBuilder.AddPrimaryKey(
          name: "PK_DoctorSpeciality",
          table: "DoctorSpeciality",
          columns: new[] { "DoctorId", "SpecialityId" });

      migrationBuilder.AddForeignKey(
          name: "FK_DoctorSpeciality_Doctors_DoctorId",
          table: "DoctorSpeciality",
          column: "DoctorId",
          principalTable: "Doctors",
          principalColumn: "DoctorId",
          onDelete: ReferentialAction.Cascade);

      migrationBuilder.AddForeignKey(
          name: "FK_DoctorSpeciality_Specialities_SpecialityId",
          table: "DoctorSpeciality",
          column: "SpecialityId",
          principalTable: "Specialities",
          principalColumn: "SpecialityId",
          onDelete: ReferentialAction.Cascade);
    }
  }
}
