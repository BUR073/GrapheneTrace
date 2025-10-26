using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GrapheneTrace.Data.Migrations
{
    /// <inheritdoc />
    public partial class DefineTableRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "GrapheneTrace");

            migrationBuilder.CreateTable(
                name: "PatientClinician",
                schema: "GrapheneTrace",
                columns: table => new
                {
                    PatientId = table.Column<int>(type: "INTEGER", nullable: false),
                    ClinicianId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PatientClinician", x => new { x.PatientId, x.ClinicianId });
                    table.ForeignKey(
                        name: "FK_PatientClinician_AspNetUsers_ClinicianId",
                        column: x => x.ClinicianId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PatientClinician_AspNetUsers_PatientId",
                        column: x => x.PatientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SensorData",
                schema: "GrapheneTrace",
                columns: table => new
                {
                    DataId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorData", x => x.DataId);
                    table.ForeignKey(
                        name: "FK_SensorData_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Alert",
                schema: "GrapheneTrace",
                columns: table => new
                {
                    AlertId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataId = table.Column<int>(type: "INTEGER", nullable: false),
                    AlertText = table.Column<string>(type: "TEXT", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alert", x => x.AlertId);
                    table.ForeignKey(
                        name: "FK_Alert_SensorData_DataId",
                        column: x => x.DataId,
                        principalSchema: "GrapheneTrace",
                        principalTable: "SensorData",
                        principalColumn: "DataId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Diagnostics",
                schema: "GrapheneTrace",
                columns: table => new
                {
                    DiagnosticsId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataId = table.Column<int>(type: "INTEGER", nullable: false),
                    PatientCondition = table.Column<string>(type: "TEXT", nullable: false),
                    Medication = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnostics", x => x.DiagnosticsId);
                    table.ForeignKey(
                        name: "FK_Diagnostics_SensorData_DataId",
                        column: x => x.DataId,
                        principalSchema: "GrapheneTrace",
                        principalTable: "SensorData",
                        principalColumn: "DataId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Feedback",
                schema: "GrapheneTrace",
                columns: table => new
                {
                    FeedbackId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataId = table.Column<int>(type: "INTEGER", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback", x => x.FeedbackId);
                    table.ForeignKey(
                        name: "FK_Feedback_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Feedback_SensorData_DataId",
                        column: x => x.DataId,
                        principalSchema: "GrapheneTrace",
                        principalTable: "SensorData",
                        principalColumn: "DataId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Heatmap",
                schema: "GrapheneTrace",
                columns: table => new
                {
                    HeatmapId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataId = table.Column<int>(type: "INTEGER", nullable: false),
                    PeakPressureIndex = table.Column<float>(type: "REAL", nullable: false),
                    ContactAreaPercent = table.Column<float>(type: "REAL", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Heatmap", x => x.HeatmapId);
                    table.ForeignKey(
                        name: "FK_Heatmap_SensorData_DataId",
                        column: x => x.DataId,
                        principalSchema: "GrapheneTrace",
                        principalTable: "SensorData",
                        principalColumn: "DataId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FeedbackReply",
                schema: "GrapheneTrace",
                columns: table => new
                {
                    feedbackReplyId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FeedbackId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<int>(type: "INTEGER", nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedbackReply", x => x.feedbackReplyId);
                    table.ForeignKey(
                        name: "FK_FeedbackReply_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FeedbackReply_Feedback_FeedbackId",
                        column: x => x.FeedbackId,
                        principalSchema: "GrapheneTrace",
                        principalTable: "Feedback",
                        principalColumn: "FeedbackId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HeatmapChunk",
                schema: "GrapheneTrace",
                columns: table => new
                {
                    ChunkId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    HeatmapId = table.Column<int>(type: "INTEGER", nullable: false),
                    ChunkNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    ChunkData = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeatmapChunk", x => x.ChunkId);
                    table.ForeignKey(
                        name: "FK_HeatmapChunk_Heatmap_HeatmapId",
                        column: x => x.HeatmapId,
                        principalSchema: "GrapheneTrace",
                        principalTable: "Heatmap",
                        principalColumn: "HeatmapId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alert_DataId",
                schema: "GrapheneTrace",
                table: "Alert",
                column: "DataId");

            migrationBuilder.CreateIndex(
                name: "IX_Diagnostics_DataId",
                schema: "GrapheneTrace",
                table: "Diagnostics",
                column: "DataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_DataId",
                schema: "GrapheneTrace",
                table: "Feedback",
                column: "DataId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_UserId",
                schema: "GrapheneTrace",
                table: "Feedback",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackReply_FeedbackId",
                schema: "GrapheneTrace",
                table: "FeedbackReply",
                column: "FeedbackId");

            migrationBuilder.CreateIndex(
                name: "IX_FeedbackReply_UserId",
                schema: "GrapheneTrace",
                table: "FeedbackReply",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Heatmap_DataId",
                schema: "GrapheneTrace",
                table: "Heatmap",
                column: "DataId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HeatmapChunk_HeatmapId",
                schema: "GrapheneTrace",
                table: "HeatmapChunk",
                column: "HeatmapId");

            migrationBuilder.CreateIndex(
                name: "IX_PatientClinician_ClinicianId",
                schema: "GrapheneTrace",
                table: "PatientClinician",
                column: "ClinicianId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorData_UserId",
                schema: "GrapheneTrace",
                table: "SensorData",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Alert",
                schema: "GrapheneTrace");

            migrationBuilder.DropTable(
                name: "Diagnostics",
                schema: "GrapheneTrace");

            migrationBuilder.DropTable(
                name: "FeedbackReply",
                schema: "GrapheneTrace");

            migrationBuilder.DropTable(
                name: "HeatmapChunk",
                schema: "GrapheneTrace");

            migrationBuilder.DropTable(
                name: "PatientClinician",
                schema: "GrapheneTrace");

            migrationBuilder.DropTable(
                name: "Feedback",
                schema: "GrapheneTrace");

            migrationBuilder.DropTable(
                name: "Heatmap",
                schema: "GrapheneTrace");

            migrationBuilder.DropTable(
                name: "SensorData",
                schema: "GrapheneTrace");
        }
    }
}
