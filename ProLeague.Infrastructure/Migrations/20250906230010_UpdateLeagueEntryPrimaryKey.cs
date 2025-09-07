using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProLeague.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLeagueEntryPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointDeductions_LeagueEntries_TeamId_LeagueId",
                table: "PointDeductions");

            migrationBuilder.DropIndex(
                name: "IX_PointDeductions_TeamId_LeagueId",
                table: "PointDeductions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeagueEntries",
                table: "LeagueEntries");

            migrationBuilder.AddColumn<string>(
                name: "Season",
                table: "PointDeductions",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Season",
                table: "LeagueEntries",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeagueEntries",
                table: "LeagueEntries",
                columns: new[] { "TeamId", "LeagueId", "Season" });

            migrationBuilder.CreateIndex(
                name: "IX_PointDeductions_TeamId_LeagueId_Season",
                table: "PointDeductions",
                columns: new[] { "TeamId", "LeagueId", "Season" });

            migrationBuilder.AddForeignKey(
                name: "FK_PointDeductions_LeagueEntries_TeamId_LeagueId_Season",
                table: "PointDeductions",
                columns: new[] { "TeamId", "LeagueId", "Season" },
                principalTable: "LeagueEntries",
                principalColumns: new[] { "TeamId", "LeagueId", "Season" },
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointDeductions_LeagueEntries_TeamId_LeagueId_Season",
                table: "PointDeductions");

            migrationBuilder.DropIndex(
                name: "IX_PointDeductions_TeamId_LeagueId_Season",
                table: "PointDeductions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LeagueEntries",
                table: "LeagueEntries");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "PointDeductions");

            migrationBuilder.AlterColumn<string>(
                name: "Season",
                table: "LeagueEntries",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LeagueEntries",
                table: "LeagueEntries",
                columns: new[] { "TeamId", "LeagueId" });

            migrationBuilder.CreateIndex(
                name: "IX_PointDeductions_TeamId_LeagueId",
                table: "PointDeductions",
                columns: new[] { "TeamId", "LeagueId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PointDeductions_LeagueEntries_TeamId_LeagueId",
                table: "PointDeductions",
                columns: new[] { "TeamId", "LeagueId" },
                principalTable: "LeagueEntries",
                principalColumns: new[] { "TeamId", "LeagueId" },
                onDelete: ReferentialAction.Cascade);
        }
    }
}
