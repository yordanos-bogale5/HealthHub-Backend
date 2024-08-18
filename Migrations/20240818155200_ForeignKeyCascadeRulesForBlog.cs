using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthHub.Migrations
{
    /// <inheritdoc />
    public partial class ForeignKeyCascadeRulesForBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogComments_Users_SenderId",
                table: "BlogComments"
            );

            migrationBuilder.DropForeignKey(name: "FK_Blogs_Users_AuthorId", table: "Blogs");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogComments_Users_SenderId",
                table: "BlogComments",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict
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
            migrationBuilder.DropForeignKey(
                name: "FK_BlogComments_Users_SenderId",
                table: "BlogComments"
            );

            migrationBuilder.DropForeignKey(name: "FK_Blogs_Users_AuthorId", table: "Blogs");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogComments_Users_SenderId",
                table: "BlogComments",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade
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
    }
}
