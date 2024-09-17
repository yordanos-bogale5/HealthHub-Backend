using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthHub.Migrations
{
    /// <inheritdoc />
    public partial class Added_TransactionReference_To_PaymentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransactionReference",
                table: "Payment",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransactionReference",
                table: "Payment");
        }
    }
}
