using System.ComponentModel.DataAnnotations.Schema;

namespace ProLeague.Domain.Entities
{
    public class Match
    {
        public int Id { get; set; }

        // Foreign Keys
        public int LeagueId { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }

        // Match Details
        public int? HomeTeamGoals { get; set; } // Nullable for matches not yet played
        public int? AwayTeamGoals { get; set; }
        public DateTime MatchDate { get; set; }
        public int MatchWeek { get; set; }
        public MatchStatus Status { get; set; } = MatchStatus.Scheduled;

        // Navigation Properties
        public League League { get; set; } = null!;
        public Team HomeTeam { get; set; } = null!;
        public Team AwayTeam { get; set; } = null!;

        [NotMapped]
        public bool IsFinished => Status == MatchStatus.Finished && HomeTeamGoals.HasValue && AwayTeamGoals.HasValue;
    }
}