// ProLeague.Domain/Entities/Match.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProLeague.Domain.Entities
{
    public class Match
    {
        public int Id { get; set; }

        public int LeagueId { get; set; }
        public League League { get; set; } = null!;

        public int HomeTeamId { get; set; }
        public Team HomeTeam { get; set; } = null!;

        public int AwayTeamId { get; set; }
        public Team AwayTeam { get; set; } = null!;

        public int? HomeTeamGoals { get; set; } // Nullable چون ممکن است بازی انجام نشده باشد
        public int? AwayTeamGoals { get; set; } // Nullable

        public DateTime MatchDate { get; set; } // تاریخ و ساعت بازی

        [Display(Name = "هفته برگزاری")]
        public int MatchWeek { get; set; }
    }
}