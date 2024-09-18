using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthHub.Migrations
{
    /// <inheritdoc />
    public partial class AddBlogTagJoinEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogTags_Blogs_BlogsBlogId",
                table: "BlogTags");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogTags_Tags_TagsTagId",
                table: "BlogTags");

            migrationBuilder.RenameColumn(
                name: "TagsTagId",
                table: "BlogTags",
                newName: "TagId");

            migrationBuilder.RenameColumn(
                name: "BlogsBlogId",
                table: "BlogTags",
                newName: "BlogId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogTags_TagsTagId",
                table: "BlogTags",
                newName: "IX_BlogTags_TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogTags_Blogs_BlogId",
                table: "BlogTags",
                column: "BlogId",
                principalTable: "Blogs",
                principalColumn: "BlogId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogTags_Tags_TagId",
                table: "BlogTags",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogTags_Blogs_BlogId",
                table: "BlogTags");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogTags_Tags_TagId",
                table: "BlogTags");

            migrationBuilder.RenameColumn(
                name: "TagId",
                table: "BlogTags",
                newName: "TagsTagId");

            migrationBuilder.RenameColumn(
                name: "BlogId",
                table: "BlogTags",
                newName: "BlogsBlogId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogTags_TagId",
                table: "BlogTags",
                newName: "IX_BlogTags_TagsTagId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogTags_Blogs_BlogsBlogId",
                table: "BlogTags",
                column: "BlogsBlogId",
                principalTable: "Blogs",
                principalColumn: "BlogId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogTags_Tags_TagsTagId",
                table: "BlogTags",
                column: "TagsTagId",
                principalTable: "Tags",
                principalColumn: "TagId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
