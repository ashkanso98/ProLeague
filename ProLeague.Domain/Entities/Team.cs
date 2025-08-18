using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProLeague.Domain.Entities
{
    public class Team
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        public string? Stadium { get; set; }
        public string? ImagePath { get; set; }
        public int FanCount { get; set; }

        // --- REMOVED ---
        // public int LeagueId { get; set; }
        // public League League { get; set; } = null!;
        // --- REMOVED ALL STATISTICS (Played, Wins, Points, etc.) ---

        // --- ADDED ---
        public ICollection<LeagueEntry> LeagueEntries { get; set; } = new List<LeagueEntry>();

        public ICollection<Player> Players { get; set; } = new List<Player>();
        public ICollection<News> News { get; set; } = new List<News>();

        [InverseProperty("HomeTeam")]
        public ICollection<Match> HomeMatches { get; set; } = new List<Match>();

        [InverseProperty("AwayTeam")]
        public ICollection<Match> AwayMatches { get; set; } = new List<Match>();
    }
}