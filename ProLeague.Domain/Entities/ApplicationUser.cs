// ProLeague.Domain/Entities/ApplicationUser.cs
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProLeague.Domain.Entities
{
    // Extend IdentityUser to add custom properties
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(100)]
        public string DisplayName { get; set; }
        public int? FavoriteTeamId { get; set; } // Nullable, user can chosen later
        public Team? FavoriteTeam { get; set; } // Navigation property
    }
}