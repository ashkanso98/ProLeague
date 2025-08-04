using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProLeague.Domain.Entities;

namespace ProLeague.Infrastructure.Data.Configurations;

public class NewsCommentConfiguration : IEntityTypeConfiguration<NewsComment>
{
    public void Configure(EntityTypeBuilder<NewsComment> builder)
    {
        builder.HasKey(nc => nc.Id);
        builder.Property(nc => nc.Content).IsRequired();

        builder.HasOne(nc => nc.News)
            .WithMany(n => n.Comments)
            .HasForeignKey(nc => nc.NewsId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(nc => nc.User)
            .WithMany()
            .HasForeignKey(nc => nc.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}