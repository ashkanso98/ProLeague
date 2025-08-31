using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProLeague.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPointDeductionSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PointDeductions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Points = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DateApplied = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TeamId = table.Column<int>(type: "int", nullable: false),
                    LeagueId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PointDeductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PointDeductions_LeagueEntries_TeamId_LeagueId",
                        columns: x => new { x.TeamId, x.LeagueId },
                        principalTable: "LeagueEntries",
                        principalColumns: new[] { "TeamId", "LeagueId" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PointDeductions_TeamId_LeagueId",
                table: "PointDeductions",
                columns: new[] { "TeamId", "LeagueId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PointDeductions");
        }
    }
}
