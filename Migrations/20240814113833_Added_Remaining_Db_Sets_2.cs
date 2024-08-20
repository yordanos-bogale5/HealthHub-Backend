using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthHub.Migrations {
  /// <inheritdoc />
  public partial class Added_Remaining_Db_Sets_2 : Migration {
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropForeignKey(
          name: "FK_Messages_File_FileId",
          table: "Messages");

      migrationBuilder.DropPrimaryKey(
          name: "PK_File",
          table: "File");

      migrationBuilder.RenameTable(
          name: "File",
          newName: "Files");

      migrationBuilder.AddPrimaryKey(
          name: "PK_Files",
          table: "Files",
          column: "FileId");

      migrationBuilder.AddForeignKey(
          name: "FK_Messages_Files_FileId",
          table: "Messages",
          column: "FileId",
          principalTable: "Files",
          principalColumn: "FileId",
          onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder) {
      migrationBuilder.DropForeignKey(
          name: "FK_Messages_Files_FileId",
          table: "Messages");

      migrationBuilder.DropPrimaryKey(
          name: "PK_Files",
          table: "Files");

      migrationBuilder.RenameTable(
          name: "Files",
          newName: "File");

      migrationBuilder.AddPrimaryKey(
          name: "PK_File",
          table: "File",
          column: "FileId");

      migrationBuilder.AddForeignKey(
          name: "FK_Messages_File_FileId",
          table: "Messages",
          column: "FileId",
          principalTable: "File",
          principalColumn: "FileId",
          onDelete: ReferentialAction.Cascade);
    }
  }
}
