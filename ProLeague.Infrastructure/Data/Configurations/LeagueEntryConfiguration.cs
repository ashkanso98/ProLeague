using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProLeague.Domain.Entities;

namespace ProLeague.Infrastructure.Data.Configurations
{
    public class LeagueEntryConfiguration : IEntityTypeConfiguration<LeagueEntry>
    {
        //public void Configure(EntityTypeBuilder<LeagueEntry> builder)
        //{
        //    builder.HasKey(le => new { le.TeamId, le.LeagueId, le.Season });
        //    // 1. Define the composite primary key
        //    // This means a unique entry is defined by the combination of a TeamId and a LeagueId.
        //    builder.HasKey(le => new { le.TeamId, le.LeagueId });

        //    // 2. Configure the relationship to the Team entity
        //    // A LeagueEntry has one Team, and a Team can have many LeagueEntries.
        //    builder.HasOne(le => le.Team)
        //        .WithMany(t => t.LeagueEntries)
        //        .HasForeignKey(le => le.TeamId)
        //        .OnDelete(DeleteBehavior.Cascade); // If a team is deleted, its league entries are also deleted.

        //    // 3. Configure the relationship to the League entity
        //    // A LeagueEntry has one League, and a League can have many TeamEntries.
        //    builder.HasOne(le => le.League)
        //        .WithMany(l => l.TeamEntries)
        //        .HasForeignKey(le => le.LeagueId)
        //        .OnDelete(DeleteBehavior.Cascade); // If a league is deleted, its team entries are also deleted.
        //}
        public void Configure(EntityTypeBuilder<LeagueEntry> builder)
        {
            // --- THIS LINE IS THE MOST IMPORTANT ---
            // It MUST define the key with all three properties.
            builder.HasKey(le => new { le.TeamId, le.LeagueId, le.Season });

            // Configure the relationship to the Team entity
            builder.HasOne(le => le.Team)
                .WithMany(t => t.LeagueEntries)
                .HasForeignKey(le => le.TeamId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the relationship to the League entity
            builder.HasOne(le => le.League)
                .WithMany(l => l.TeamEntries)
                .HasForeignKey(le => le.LeagueId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}