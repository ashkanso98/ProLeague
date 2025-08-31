using System.ComponentModel.DataAnnotations;

namespace ProLeague.Domain.Entities
{
    public class PointDeduction
    {
        public int Id { get; set; }

        [Required]
        public int Points { get; set; } // The number of points to deduct

        [Required]
        [StringLength(200)]
        public string Reason { get; set; } // e.g., "Financial Fair Play Violation"

        public DateTime DateApplied { get; set; } = DateTime.UtcNow;

        // Foreign keys to link to a specific team in a specific league
        public int TeamId { get; set; }
        public int LeagueId { get; set; }

        // Navigation Property to the LeagueEntry
        public LeagueEntry LeagueEntry { get; set; } = null!;
    }
}