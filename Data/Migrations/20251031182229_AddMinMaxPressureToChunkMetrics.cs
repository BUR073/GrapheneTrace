using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrapheneTrace.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMinMaxPressureToChunkMetrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "MaxPressure",
                table: "ChunkMetrics",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "MinPressure",
                table: "ChunkMetrics",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxPressure",
                table: "ChunkMetrics");

            migrationBuilder.DropColumn(
                name: "MinPressure",
                table: "ChunkMetrics");
        }
    }
}
