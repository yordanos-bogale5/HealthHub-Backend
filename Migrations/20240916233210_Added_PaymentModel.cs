using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthHub.Migrations
{
    /// <inheritdoc />
    public partial class Added_PaymentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Doctors_DoctorId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Payment_Patients_PatientId",
                table: "Payment");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Payment_PaymentId",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payment",
                table: "Payment");

            migrationBuilder.RenameTable(
                name: "Payment",
                newName: "Payments");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_PatientId",
                table: "Payments",
                newName: "IX_Payments_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Payment_DoctorId",
                table: "Payments",
                newName: "IX_Payments_DoctorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payments",
                table: "Payments",
                column: "PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Doctors_DoctorId",
                table: "Payments",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "DoctorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Patients_PatientId",
                table: "Payments",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Payments_PaymentId",
                table: "Reviews",
                column: "PaymentId",
                principalTable: "Payments",
                principalColumn: "PaymentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Doctors_DoctorId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Patients_PatientId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Reviews_Payments_PaymentId",
                table: "Reviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Payments",
                table: "Payments");

            migrationBuilder.RenameTable(
                name: "Payments",
                newName: "Payment");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_PatientId",
                table: "Payment",
                newName: "IX_Payment_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_DoctorId",
                table: "Payment",
                newName: "IX_Payment_DoctorId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Payment",
                table: "Payment",
                column: "PaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Doctors_DoctorId",
                table: "Payment",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "DoctorId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payment_Patients_PatientId",
                table: "Payment",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "PatientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Reviews_Payment_PaymentId",
                table: "Reviews",
                column: "PaymentId",
                principalTable: "Payment",
                principalColumn: "PaymentId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
