using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrapheneTrace.Data.Migrations
{
    /// <inheritdoc />
    public partial class changeRelationForFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_SensorData_DataId",
                table: "Feedback");

            migrationBuilder.RenameColumn(
                name: "DataId",
                table: "Feedback",
                newName: "ChunkId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedback_DataId",
                table: "Feedback",
                newName: "IX_Feedback_ChunkId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_HeatmapChunk_ChunkId",
                table: "Feedback",
                column: "ChunkId",
                principalTable: "HeatmapChunk",
                principalColumn: "ChunkId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feedback_HeatmapChunk_ChunkId",
                table: "Feedback");

            migrationBuilder.RenameColumn(
                name: "ChunkId",
                table: "Feedback",
                newName: "DataId");

            migrationBuilder.RenameIndex(
                name: "IX_Feedback_ChunkId",
                table: "Feedback",
                newName: "IX_Feedback_DataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Feedback_SensorData_DataId",
                table: "Feedback",
                column: "DataId",
                principalTable: "SensorData",
                principalColumn: "DataId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
