using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrapheneTrace.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateDeletionRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_AspNetUsers_UserId",
                table: "Feedback");

            migrationBuilder.DropForeignKey(
                name: "FK_FeedbackReply_AspNetUsers_UserId",
                table: "FeedbackReply");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientClinician_AspNetUsers_ClinicianId",
                table: "PatientClinician");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientClinician_AspNetUsers_PatientId",
                table: "PatientClinician");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_AspNetUsers_UserId",
                table: "Feedback",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FeedbackReply_AspNetUsers_UserId",
                table: "FeedbackReply",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientClinician_AspNetUsers_ClinicianId",
                table: "PatientClinician",
                column: "ClinicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientClinician_AspNetUsers_PatientId",
                table: "PatientClinician",
                column: "PatientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_AspNetUsers_UserId",
                table: "Feedback");

            migrationBuilder.DropForeignKey(
                name: "FK_FeedbackReply_AspNetUsers_UserId",
                table: "FeedbackReply");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientClinician_AspNetUsers_ClinicianId",
                table: "PatientClinician");

            migrationBuilder.DropForeignKey(
                name: "FK_PatientClinician_AspNetUsers_PatientId",
                table: "PatientClinician");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_AspNetUsers_UserId",
                table: "Feedback",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FeedbackReply_AspNetUsers_UserId",
                table: "FeedbackReply",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientClinician_AspNetUsers_ClinicianId",
                table: "PatientClinician",
                column: "ClinicianId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PatientClinician_AspNetUsers_PatientId",
                table: "PatientClinician",
                column: "PatientId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
