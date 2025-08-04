using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProLeague.Domain.Entities;

namespace ProLeague.Infrastructure.Data.Configurations;

public class NewsConfiguration : IEntityTypeConfiguration<News>
{
    public void Configure(EntityTypeBuilder<News> builder)
    {
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Title).IsRequired().HasMaxLength(200);
        builder.Property(n => n.Content).IsRequired();

        // روابط چند به چند
        builder.HasMany(n => n.RelatedLeagues).WithMany(l => l.News);
        builder.HasMany(n => n.RelatedTeams).WithMany(t => t.News);
        builder.HasMany(n => n.RelatedPlayers).WithMany(p => p.News);
    }
}