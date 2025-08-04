using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProLeague.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMoreFutures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewsPlayer_News_NewsId",
                table: "NewsPlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_NewsPlayer_Player_PlayerId",
                table: "NewsPlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_NewsTeam_News_NewsId",
                table: "NewsTeam");

            migrationBuilder.DropForeignKey(
                name: "FK_NewsTeam_Team_TeamId",
                table: "NewsTeam");

            migrationBuilder.DropTable(
                name: "NewsLeague");

            migrationBuilder.DropColumn(
                name: "Draws",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "GoalsAgainst",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "GoalsFor",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Losses",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Played",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "Wins",
                table: "Teams");

            migrationBuilder.RenameColumn(
                name: "TeamId",
                table: "NewsTeam",
                newName: "RelatedTeamsId");

            migrationBuilder.RenameIndex(
                name: "IX_NewsTeam_TeamId",
                table: "NewsTeam",
                newName: "IX_NewsTeam_RelatedTeamsId");

            migrationBuilder.RenameColumn(
                name: "PlayerId",
                table: "NewsPlayer",
                newName: "RelatedPlayersId");

            migrationBuilder.RenameIndex(
                name: "IX_NewsPlayer_PlayerId",
                table: "NewsPlayer",
                newName: "IX_NewsPlayer_RelatedPlayersId");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "NewsComments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(128)",
                oldMaxLength: 128);

            migrationBuilder.CreateTable(
                name: "LeagueNews",
                columns: table => new
                {
                    NewsId = table.Column<int>(type: "int", nullable: false),
                    RelatedLeaguesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeagueNews", x => new { x.NewsId, x.RelatedLeaguesId });
                    table.ForeignKey(
                        name: "FK_LeagueNews_Leagues_RelatedLeaguesId",
                        column: x => x.RelatedLeaguesId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LeagueNews_News_NewsId",
                        column: x => x.NewsId,
                        principalTable: "News",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Matches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LeagueId = table.Column<int>(type: "int", nullable: false),
                    HomeTeamId = table.Column<int>(type: "int", nullable: false),
                    AwayTeamId = table.Column<int>(type: "int", nullable: false),
                    HomeTeamGoals = table.Column<int>(type: "int", nullable: true),
                    AwayTeamGoals = table.Column<int>(type: "int", nullable: true),
                    MatchDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MatchWeek = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matches_Leagues_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Matches_Teams_AwayTeamId",
                        column: x => x.AwayTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Matches_Teams_HomeTeamId",
                        column: x => x.HomeTeamId,
                        principalTable: "Teams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LeagueNews_RelatedLeaguesId",
                table: "LeagueNews",
                column: "RelatedLeaguesId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_AwayTeamId",
                table: "Matches",
                column: "AwayTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_HomeTeamId",
                table: "Matches",
                column: "HomeTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Matches_LeagueId",
                table: "Matches",
                column: "LeagueId");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsPlayer_News_NewsId",
                table: "NewsPlayer",
                column: "NewsId",
                principalTable: "News",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NewsPlayer_Players_RelatedPlayersId",
                table: "NewsPlayer",
                column: "RelatedPlayersId",
                principalTable: "Players",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NewsTeam_News_NewsId",
                table: "NewsTeam",
                column: "NewsId",
                principalTable: "News",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_NewsTeam_Teams_RelatedTeamsId",
                table: "NewsTeam",
                column: "RelatedTeamsId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewsPlayer_News_NewsId",
                table: "NewsPlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_NewsPlayer_Players_RelatedPlayersId",
                table: "NewsPlayer");

            migrationBuilder.DropForeignKey(
                name: "FK_NewsTeam_News_NewsId",
                table: "NewsTeam");

            migrationBuilder.DropForeignKey(
                name: "FK_NewsTeam_Teams_RelatedTeamsId",
                table: "NewsTeam");

            migrationBuilder.DropTable(
                name: "LeagueNews");

            migrationBuilder.DropTable(
                name: "Matches");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "NewsComments");

            migrationBuilder.RenameColumn(
                name: "RelatedTeamsId",
                table: "NewsTeam",
                newName: "TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_NewsTeam_RelatedTeamsId",
                table: "NewsTeam",
                newName: "IX_NewsTeam_TeamId");

            migrationBuilder.RenameColumn(
                name: "RelatedPlayersId",
                table: "NewsPlayer",
                newName: "PlayerId");

            migrationBuilder.RenameIndex(
                name: "IX_NewsPlayer_RelatedPlayersId",
                table: "NewsPlayer",
                newName: "IX_NewsPlayer_PlayerId");

            migrationBuilder.AddColumn<int>(
                name: "Draws",
                table: "Teams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoalsAgainst",
                table: "Teams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "GoalsFor",
                table: "Teams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Losses",
                table: "Teams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Played",
                table: "Teams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Wins",
                table: "Teams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "AspNetUserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.CreateTable(
                name: "NewsLeague",
                columns: table => new
                {
                    LeagueId = table.Column<int>(type: "int", nullable: false),
                    NewsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NewsLeague", x => new { x.LeagueId, x.NewsId });
                    table.ForeignKey(
                        name: "FK_NewsLeague_League_LeagueId",
                        column: x => x.LeagueId,
                        principalTable: "Leagues",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_NewsLeague_News_NewsId",
                        column: x => x.NewsId,
                        principalTable: "News",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NewsLeague_NewsId",
                table: "NewsLeague",
                column: "NewsId");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsPlayer_News_NewsId",
                table: "NewsPlayer",
                column: "NewsId",
                principalTable: "News",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsPlayer_Player_PlayerId",
                table: "NewsPlayer",
                column: "PlayerId",
                principalTable: "Players",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsTeam_News_NewsId",
                table: "NewsTeam",
                column: "NewsId",
                principalTable: "News",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsTeam_Team_TeamId",
                table: "NewsTeam",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }
    }
}
