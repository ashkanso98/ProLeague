using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProLeague.Domain.Entities;

namespace ProLeague.Infrastructure.Data.Configurations
{
    public class PointDeductionConfiguration : IEntityTypeConfiguration<PointDeduction>
    {
        public void Configure(EntityTypeBuilder<PointDeduction> builder)
        {
            builder.HasKey(d => d.Id);

            // Configure the relationship with LeagueEntry
            // A PointDeduction belongs to one LeagueEntry, which has a composite key.
            builder.HasOne(d => d.LeagueEntry)
                .WithMany(le => le.Deductions)
                .HasForeignKey(d => new { d.TeamId, d.LeagueId });
        }
    }
}