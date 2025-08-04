using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProLeague.Infrastructure.Data.Configurations;

public class NewsImageConfiguration : IEntityTypeConfiguration<NewsImage>
{
    public void Configure(EntityTypeBuilder<NewsImage> builder)
    {
        builder.HasKey(ni => ni.Id);
        builder.Property(ni => ni.ImagePath).IsRequired();
        builder.HasOne(ni => ni.News)
            .WithMany(n => n.Images)
            .HasForeignKey(ni => ni.NewsId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}