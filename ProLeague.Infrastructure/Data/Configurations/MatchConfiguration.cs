using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProLeague.Domain.Entities;

namespace ProLeague.Infrastructure.Data.Configurations;

public class MatchConfiguration : IEntityTypeConfiguration<Match>
{
    public void Configure(EntityTypeBuilder<Match> builder)
    {
        builder.HasKey(m => m.Id);

        // رابطه با لیگ
        builder.HasOne(m => m.League)
            .WithMany(l => l.Matches)
            .HasForeignKey(m => m.LeagueId)
            .OnDelete(DeleteBehavior.Cascade);

        // رابطه با تیم میزبان
        builder.HasOne(m => m.HomeTeam)
            .WithMany(t => t.HomeMatches)
            .HasForeignKey(m => m.HomeTeamId)
            .OnDelete(DeleteBehavior.Restrict); // جلوگیری از حذف تیمی که بازی دارد

        // رابطه با تیم میهمان
        builder.HasOne(m => m.AwayTeam)
            .WithMany(t => t.AwayMatches)
            .HasForeignKey(m => m.AwayTeamId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}