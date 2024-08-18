using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthHub.Migrations
{
    /// <inheritdoc />
    public partial class Update_BlogModel_ForeignKeySetupAppropriatelyFollowingNamingConvention
        : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Blogs_Users_UserId", table: "Blogs");

            migrationBuilder.DropIndex(name: "IX_Blogs_UserId", table: "Blogs");

            migrationBuilder.DropColumn(name: "UserId", table: "Blogs");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_AuthorId",
                table: "Blogs",
                column: "AuthorId"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Users_AuthorId",
                table: "Blogs",
                column: "AuthorId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.NoAction
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(name: "FK_Blogs_Users_AuthorId", table: "Blogs");

            migrationBuilder.DropIndex(name: "IX_Blogs_AuthorId", table: "Blogs");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Blogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );

            migrationBuilder.CreateIndex(name: "IX_Blogs_UserId", table: "Blogs", column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Users_UserId",
                table: "Blogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.NoAction
            );
        }
    }
}
