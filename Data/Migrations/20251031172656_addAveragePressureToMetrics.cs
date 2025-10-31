using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrapheneTrace.Data.Migrations
{
    /// <inheritdoc />
    public partial class addAveragePressureToMetrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "AveragePressure",
                table: "ChunkMetrics",
                type: "REAL",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AveragePressure",
                table: "ChunkMetrics");
        }
    }
}
