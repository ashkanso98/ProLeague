using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProLeague.Domain.Entities;

namespace ProLeague.Infrastructure.Data.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasOne(au => au.FavoriteTeam)
            .WithMany()
            .HasForeignKey(au => au.FavoriteTeamId)
            .OnDelete(DeleteBehavior.SetNull); // با حذف تیم، تیم محبوب کاربر Null می‌شود
    }
}