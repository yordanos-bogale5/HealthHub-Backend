using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthHub.Migrations
{
    /// <inheritdoc />
    public partial class Added_M2MConstraint_ToBlog_And_Tag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogTag_Blogs_BlogsBlogId",
                table: "BlogTag");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogTag_Tags_TagsTagId",
                table: "BlogTag");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogTag",
                table: "BlogTag");

            migrationBuilder.RenameTable(
                name: "BlogTag",
                newName: "BlogTags");

            migrationBuilder.RenameIndex(
                name: "IX_BlogTag_TagsTagId",
                table: "BlogTags",
                newName: "IX_BlogTags_TagsTagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogTags",
                table: "BlogTags",
                columns: new[] { "BlogsBlogId", "TagsTagId" });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogTags_Blogs_BlogsBlogId",
                table: "BlogTags");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogTags_Tags_TagsTagId",
                table: "BlogTags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogTags",
                table: "BlogTags");

            migrationBuilder.RenameTable(
                name: "BlogTags",
                newName: "BlogTag");

            migrationBuilder.RenameIndex(
                name: "IX_BlogTags_TagsTagId",
                table: "BlogTag",
                newName: "IX_BlogTag_TagsTagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogTag",
                table: "BlogTag",
                columns: new[] { "BlogsBlogId", "TagsTagId" });

            migrationBuilder.AddForeignKey(
                name: "FK_BlogTag_Blogs_BlogsBlogId",
                table: "BlogTag",
                column: "BlogsBlogId",
                principalTable: "Blogs",
                principalColumn: "BlogId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogTag_Tags_TagsTagId",
                table: "BlogTag",
                column: "TagsTagId",
                principalTable: "Tags",
                principalColumn: "TagId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
