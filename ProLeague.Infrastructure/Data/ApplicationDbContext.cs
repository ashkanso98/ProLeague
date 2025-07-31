using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using ProLeague.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace ProLeague.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<League> Leagues { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<NewsImage> NewsImages { get; set; }
        public DbSet<NewsComment> NewsComments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure many-to-many relationships for News
            builder.Entity<News>()
                .HasMany(n => n.RelatedLeagues)
                .WithMany(l => l.News)
                .UsingEntity<Dictionary<string, object>>(
                    "NewsLeague",
                    j => j
                        .HasOne<League>()
                        .WithMany()
                        .HasForeignKey("LeagueId")
                        .HasConstraintName("FK_NewsLeague_League_LeagueId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j => j
                        .HasOne<News>()
                        .WithMany()
                        .HasForeignKey("NewsId")
                        .HasConstraintName("FK_NewsLeague_News_NewsId")
                        .OnDelete(DeleteBehavior.ClientSetNull));

            builder.Entity<News>()
                .HasMany(n => n.RelatedTeams)
                .WithMany(t => t.News)
                .UsingEntity<Dictionary<string, object>>(
                    "NewsTeam",
                    j => j
                        .HasOne<Team>()
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .HasConstraintName("FK_NewsTeam_Team_TeamId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j => j
                        .HasOne<News>()
                        .WithMany()
                        .HasForeignKey("NewsId")
                        .HasConstraintName("FK_NewsTeam_News_NewsId")
                        .OnDelete(DeleteBehavior.ClientSetNull));

            builder.Entity<News>()
                .HasMany(n => n.RelatedPlayers)
                .WithMany(p => p.News)
                .UsingEntity<Dictionary<string, object>>(
                    "NewsPlayer",
                    j => j
                        .HasOne<Player>()
                        .WithMany()
                        .HasForeignKey("PlayerId")
                        .HasConstraintName("FK_NewsPlayer_Player_PlayerId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    j => j
                        .HasOne<News>()
                        .WithMany()
                        .HasForeignKey("NewsId")
                        .HasConstraintName("FK_NewsPlayer_News_NewsId")
                        .OnDelete(DeleteBehavior.ClientSetNull));

            // Configure Team-League relationship
            builder.Entity<Team>()
                .HasOne(t => t.League)
                .WithMany(l => l.Teams)
                .HasForeignKey(t => t.LeagueId)
                .OnDelete(DeleteBehavior.Cascade); // If a league is deleted, teams are deleted

            // Configure Player-Team relationship
            builder.Entity<Player>()
                .HasOne(p => p.Team)
                .WithMany(t => t.Players)
                .HasForeignKey(p => p.TeamId)
                .OnDelete(DeleteBehavior.Cascade); // If a team is deleted, players are deleted

            // Configure NewsComment-News relationship
            builder.Entity<NewsComment>()
                .HasOne(nc => nc.News)
                .WithMany(n => n.Comments)
                .HasForeignKey(nc => nc.NewsId)
                .OnDelete(DeleteBehavior.Cascade); // If news is deleted, comments are deleted

            // Configure NewsComment-User relationship
            builder.Entity<NewsComment>()
                .HasOne(nc => nc.User)
                .WithMany() // ApplicationUser doesn't have a navigation property back to comments
                .HasForeignKey(nc => nc.UserId)
                .OnDelete(DeleteBehavior.Cascade); // If user is deleted, comments are deleted

            // Configure ApplicationUser-Team relationship (Favorite Team)
            builder.Entity<ApplicationUser>()
                .HasOne(au => au.FavoriteTeam)
                .WithMany() // Team doesn't have a navigation property back to users
                .HasForeignKey(au => au.FavoriteTeamId)
                .OnDelete(DeleteBehavior.SetNull); // If team is deleted, favorite team ID is set to null

            // Seed data for initial setup (optional)
            // builder.Entity<League>().HasData(new League { Id = 1, Name = "Premier League", Country = "England" });
            // Add more seed data as needed
        }
    }
}

