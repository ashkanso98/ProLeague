using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProLeague.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsImportantFlagToTeam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsImportant",
                table: "Teams",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsImportant",
                table: "Teams");
        }
    }
}
