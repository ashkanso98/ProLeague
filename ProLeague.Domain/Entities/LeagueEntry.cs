using System.ComponentModel.DataAnnotations.Schema;

namespace ProLeague.Domain.Entities
{
    public class LeagueEntry
    {
        // Composite Primary Key
        public int TeamId { get; set; }
        public int LeagueId { get; set; }

        // Navigation Properties
        public Team Team { get; set; } = null!;
        public League League { get; set; } = null!;

        // Statistics for this team IN this league
        public int Played { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }

        [NotMapped]
        public int GoalDifference => GoalsFor - GoalsAgainst;
        [NotMapped]
        public int Points => (Wins * 3) + Draws;
    }
}