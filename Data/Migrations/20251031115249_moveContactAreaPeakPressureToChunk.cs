using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrapheneTrace.Data.Migrations
{
    /// <inheritdoc />
    public partial class moveContactAreaPeakPressureToChunk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactAreaPercent",
                table: "Heatmap");

            migrationBuilder.DropColumn(
                name: "PeakPressureIndex",
                table: "Heatmap");

            migrationBuilder.AddColumn<float>(
                name: "ContactAreaPercent",
                table: "HeatmapChunk",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "PeakPressureIndex",
                table: "HeatmapChunk",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactAreaPercent",
                table: "HeatmapChunk");

            migrationBuilder.DropColumn(
                name: "PeakPressureIndex",
                table: "HeatmapChunk");

            migrationBuilder.AddColumn<float>(
                name: "ContactAreaPercent",
                table: "Heatmap",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "PeakPressureIndex",
                table: "Heatmap",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
