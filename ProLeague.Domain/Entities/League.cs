using System.ComponentModel.DataAnnotations;

namespace ProLeague.Domain.Entities
{
    public class League
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Country { get; set; }

        public string? ImagePath { get; set; }

        // --- REMOVED ---
        // public ICollection<Team> Teams { get; set; } = new List<Team>();

        // --- ADDED ---
        public ICollection<LeagueEntry> TeamEntries { get; set; } = new List<LeagueEntry>();

        public ICollection<News> News { get; set; } = new List<News>();
        public ICollection<Match> Matches { get; set; } = new List<Match>();
    }
}