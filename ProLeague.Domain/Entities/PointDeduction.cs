using System.ComponentModel.DataAnnotations;

namespace ProLeague.Domain.Entities
{
    public class PointDeduction
    {
        public int Id { get; set; }
        public int Points { get; set; }
        [StringLength(200)]
        public string Reason { get; set; }
        public DateTime DateApplied { get; set; } = DateTime.UtcNow;

        // Foreign keys to link to a specific team in a specific league for a specific season
        public int TeamId { get; set; }
        public int LeagueId { get; set; }
        [StringLength(10)]
        public string Season { get; set; } = null!; // <-- ADD THIS PROPERTY

        // Navigation Property to the LeagueEntry
        public LeagueEntry LeagueEntry { get; set; } = null!;
    }
}
