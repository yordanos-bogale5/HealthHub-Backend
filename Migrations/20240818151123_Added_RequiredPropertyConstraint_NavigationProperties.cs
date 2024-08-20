using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthHub.Migrations {
  /// <inheritdoc />
  public partial class Added_RequiredPropertyConstraint_NavigationProperties : Migration {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropForeignKey(name: "FK_Blogs_Users_UserId", table: "Blogs");

      migrationBuilder.AlterColumn<Guid>(
          name: "UserId",
          table: "Blogs",
          type: "uniqueidentifier",
          nullable: false,
          defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
          oldClrType: typeof(Guid),
          oldType: "uniqueidentifier",
          oldNullable: true
      );

      migrationBuilder.AddForeignKey(
          name: "FK_Blogs_Users_UserId",
          table: "Blogs",
          column: "UserId",
          principalTable: "Users",
          principalColumn: "UserId",
          onDelete: ReferentialAction.NoAction
      );
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropForeignKey(name: "FK_Blogs_Users_UserId", table: "Blogs");

      migrationBuilder.AlterColumn<Guid>(
          name: "UserId",
          table: "Blogs",
          type: "uniqueidentifier",
          nullable: true,
          oldClrType: typeof(Guid),
          oldType: "uniqueidentifier"
      );

      migrationBuilder.AddForeignKey(
          name: "FK_Blogs_Users_UserId",
          table: "Blogs",
          column: "UserId",
          principalTable: "Users",
          principalColumn: "UserId"
      );
    }
  }
}
