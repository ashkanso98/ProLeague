// ProLeague.Infrastructure/Data/ApplicationDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProLeague.Domain.Entities;
using System.Reflection;

namespace ProLeague.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // لیست تمام انتیتی‌های اصلی پروژه
        public DbSet<League> Leagues { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<NewsImage> NewsImages { get; set; }
        public DbSet<NewsComment> NewsComments { get; set; }
        public DbSet<Match> Matches { get; set; } // DbSet جدید
        public DbSet<LeagueEntry> LeagueEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // این متد به صورت خودکار تمام کلاس‌های کانفیگ را از این Assembly پیدا و اعمال می‌کند
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}