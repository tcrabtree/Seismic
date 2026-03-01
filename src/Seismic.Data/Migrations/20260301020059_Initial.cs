using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Seismic.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SiteId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    MonitorId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EventTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Distance = table.Column<double>(type: "float", nullable: false),
                    WeightPerDelay = table.Column<double>(type: "float", nullable: false),
                    ScaledDistance = table.Column<double>(type: "float", nullable: false),
                    RawCsvPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsReviewed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "MonitorHealths",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MonitorId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HealthScore = table.Column<double>(type: "float", nullable: false),
                    R2Value = table.Column<double>(type: "float", nullable: false),
                    EnergySymmetry = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitorHealths", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WaveformPoints",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Time = table.Column<double>(type: "float", nullable: false),
                    R = table.Column<double>(type: "float", nullable: false),
                    T = table.Column<double>(type: "float", nullable: false),
                    V = table.Column<double>(type: "float", nullable: false),
                    A = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WaveformPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WaveformPoints_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "EventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventTimestamp",
                table: "Events",
                column: "EventTimestamp");

            migrationBuilder.CreateIndex(
                name: "IX_Events_SiteId",
                table: "Events",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_MonitorHealths_MonitorId_Date",
                table: "MonitorHealths",
                columns: new[] { "MonitorId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_WaveformPoints_EventId",
                table: "WaveformPoints",
                column: "EventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonitorHealths");

            migrationBuilder.DropTable(
                name: "WaveformPoints");

            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
