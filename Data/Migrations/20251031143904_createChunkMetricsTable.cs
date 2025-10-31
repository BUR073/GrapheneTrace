using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrapheneTrace.Data.Migrations
{
    /// <inheritdoc />
    public partial class createChunkMetricsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContactAreaPercent",
                table: "HeatmapChunk");

            migrationBuilder.DropColumn(
                name: "PeakPressureIndex",
                table: "HeatmapChunk");

            migrationBuilder.CreateTable(
                name: "ChunkMetrics",
                columns: table => new
                {
                    ChunkId = table.Column<int>(type: "INTEGER", nullable: false),
                    PeakPressureIndex = table.Column<float>(type: "REAL", nullable: false),
                    ContactAreaPercent = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChunkMetrics", x => x.ChunkId);
                    table.ForeignKey(
                        name: "FK_ChunkMetrics_HeatmapChunk_ChunkId",
                        column: x => x.ChunkId,
                        principalTable: "HeatmapChunk",
                        principalColumn: "ChunkId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChunkMetrics");

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
    }
}
