using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrapheneTrace.Data.Migrations
{
    /// <inheritdoc />
    public partial class smallChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "SensorData",
                schema: "GrapheneTrace",
                newName: "SensorData");

            migrationBuilder.RenameTable(
                name: "PatientClinician",
                schema: "GrapheneTrace",
                newName: "PatientClinician");

            migrationBuilder.RenameTable(
                name: "HeatmapChunk",
                schema: "GrapheneTrace",
                newName: "HeatmapChunk");

            migrationBuilder.RenameTable(
                name: "Heatmap",
                schema: "GrapheneTrace",
                newName: "Heatmap");

            migrationBuilder.RenameTable(
                name: "FeedbackReply",
                schema: "GrapheneTrace",
                newName: "FeedbackReply");

            migrationBuilder.RenameTable(
                name: "Feedback",
                schema: "GrapheneTrace",
                newName: "Feedback");

            migrationBuilder.RenameTable(
                name: "Diagnostics",
                schema: "GrapheneTrace",
                newName: "Diagnostics");

            migrationBuilder.RenameTable(
                name: "Alert",
                schema: "GrapheneTrace",
                newName: "Alert");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "GrapheneTrace");

            migrationBuilder.RenameTable(
                name: "SensorData",
                newName: "SensorData",
                newSchema: "GrapheneTrace");

            migrationBuilder.RenameTable(
                name: "PatientClinician",
                newName: "PatientClinician",
                newSchema: "GrapheneTrace");

            migrationBuilder.RenameTable(
                name: "HeatmapChunk",
                newName: "HeatmapChunk",
                newSchema: "GrapheneTrace");

            migrationBuilder.RenameTable(
                name: "Heatmap",
                newName: "Heatmap",
                newSchema: "GrapheneTrace");

            migrationBuilder.RenameTable(
                name: "FeedbackReply",
                newName: "FeedbackReply",
                newSchema: "GrapheneTrace");

            migrationBuilder.RenameTable(
                name: "Feedback",
                newName: "Feedback",
                newSchema: "GrapheneTrace");

            migrationBuilder.RenameTable(
                name: "Diagnostics",
                newName: "Diagnostics",
                newSchema: "GrapheneTrace");

            migrationBuilder.RenameTable(
                name: "Alert",
                newName: "Alert",
                newSchema: "GrapheneTrace");
        }
    }
}
