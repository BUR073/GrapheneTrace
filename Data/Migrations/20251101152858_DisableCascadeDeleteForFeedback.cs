using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrapheneTrace.Data.Migrations
{
    /// <inheritdoc />
    public partial class DisableCascadeDeleteForFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_HeatmapChunk_ChunkId",
                table: "Feedback");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_HeatmapChunk_ChunkId",
                table: "Feedback",
                column: "ChunkId",
                principalTable: "HeatmapChunk",
                principalColumn: "ChunkId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_HeatmapChunk_ChunkId",
                table: "Feedback");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_HeatmapChunk_ChunkId",
                table: "Feedback",
                column: "ChunkId",
                principalTable: "HeatmapChunk",
                principalColumn: "ChunkId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
