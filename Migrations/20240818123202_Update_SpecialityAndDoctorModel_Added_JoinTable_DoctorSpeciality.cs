using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthHub.Migrations {
  /// <inheritdoc />
  public partial class Update_SpecialityAndDoctorModel_Added_JoinTable_DoctorSpeciality : Migration {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropForeignKey(
          name: "FK_Specialities_Doctors_DoctorId",
          table: "Specialities");

      migrationBuilder.DropIndex(
          name: "IX_Specialities_DoctorId",
          table: "Specialities");

      migrationBuilder.DropColumn(
          name: "DoctorId",
          table: "Specialities");

      migrationBuilder.CreateTable(
          name: "DoctorSpeciality",
          columns: table => new {
            DoctorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            SpecialityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
          },
          constraints: table => {
            table.PrimaryKey("PK_DoctorSpeciality", x => new { x.DoctorId, x.SpecialityId });
            table.ForeignKey(
                      name: "FK_DoctorSpeciality_Doctors_DoctorId",
                      column: x => x.DoctorId,
                      principalTable: "Doctors",
                      principalColumn: "DoctorId",
                      onDelete: ReferentialAction.Cascade);
            table.ForeignKey(
                      name: "FK_DoctorSpeciality_Specialities_SpecialityId",
                      column: x => x.SpecialityId,
                      principalTable: "Specialities",
                      principalColumn: "SpecialityId",
                      onDelete: ReferentialAction.Cascade);
          });

      migrationBuilder.CreateIndex(
          name: "IX_DoctorSpeciality_SpecialityId",
          table: "DoctorSpeciality",
          column: "SpecialityId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropTable(
          name: "DoctorSpeciality");

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
  }
}
