using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProLeague.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSeasonsToCompetitions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Round",
                table: "Matches",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Season",
                table: "Matches",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Season",
                table: "LeagueEntries",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Round",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "Matches");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "LeagueEntries");
        }
    }
}
