// ProLeague.Domain/Entities/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;

namespace ProLeague.Domain.Entities
{
    // Extend IdentityUser to add custom properties
    public class ApplicationUser : IdentityUser
    {
        public int? FavoriteTeamId { get; set; } // Nullable, user can chosen later
        public Team? FavoriteTeam { get; set; } // Navigation property
    }
}