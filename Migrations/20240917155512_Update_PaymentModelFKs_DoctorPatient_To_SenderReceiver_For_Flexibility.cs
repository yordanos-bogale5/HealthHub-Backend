using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthHub.Migrations
{
    /// <inheritdoc />
    public partial class Update_PaymentModelFKs_DoctorPatient_To_SenderReceiver_For_Flexibility : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Doctors_DoctorId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Patients_PatientId",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "PatientId",
                table: "Payments",
                newName: "SenderId");

            migrationBuilder.RenameColumn(
                name: "DoctorId",
                table: "Payments",
                newName: "ReceiverId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_PatientId",
                table: "Payments",
                newName: "IX_Payments_SenderId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_DoctorId",
                table: "Payments",
                newName: "IX_Payments_ReceiverId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_ReceiverId",
                table: "Payments",
                column: "ReceiverId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Users_SenderId",
                table: "Payments",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_ReceiverId",
                table: "Payments");

            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Users_SenderId",
                table: "Payments");

            migrationBuilder.RenameColumn(
                name: "SenderId",
                table: "Payments",
                newName: "PatientId");

            migrationBuilder.RenameColumn(
                name: "ReceiverId",
                table: "Payments",
                newName: "DoctorId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_SenderId",
                table: "Payments",
                newName: "IX_Payments_PatientId");

            migrationBuilder.RenameIndex(
                name: "IX_Payments_ReceiverId",
                table: "Payments",
                newName: "IX_Payments_DoctorId");

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
        }
    }
}
