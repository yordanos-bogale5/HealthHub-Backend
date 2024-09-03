using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthHub.Migrations
{
    /// <inheritdoc />
    public partial class Added_StartDate_And_EndDate_To_EducationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GraduationDate",
                table: "Educations",
                newName: "StartDate");

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndDate",
                table: "Educations",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Educations");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Educations",
                newName: "GraduationDate");
        }
    }
}
