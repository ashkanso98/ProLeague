// ProLeague.Domain/Entities/Team.cs
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

        public int LeagueId { get; set; }
        public League League { get; set; } = null!;

        public ICollection<Player> Players { get; set; } = new List<Player>();
        public ICollection<News> News { get; set; } = new List<News>();

        // --- پراپرتی‌های جدید برای بازی‌های خانگی و خارج از خانه ---
        [InverseProperty("HomeTeam")]
        public ICollection<Match> HomeMatches { get; set; } = new List<Match>();

        [InverseProperty("AwayTeam")]
        public ICollection<Match> AwayMatches { get; set; } = new List<Match>();

        // --- فیلدهای آمار به صورت Read-only و محاسبه‌شده ---
        [NotMapped] // این فیلدها در دیتابیس ذخیره نمی‌شوند
        public int Played => FinishedMatches.Count();

        [NotMapped]
        public int Wins => FinishedMatches.Count(m => (m.HomeTeamId == Id && m.HomeTeamGoals > m.AwayTeamGoals) || (m.AwayTeamId == Id && m.AwayTeamGoals > m.HomeTeamGoals));

        [NotMapped]
        public int Draws => FinishedMatches.Count(m => m.HomeTeamGoals == m.AwayTeamGoals);

        [NotMapped]
        public int Losses => Played - Wins - Draws;

        [NotMapped]
        public int GoalsFor => FinishedMatches.Where(m => m.HomeTeamId == Id).Sum(m => m.HomeTeamGoals ?? 0) + FinishedMatches.Where(m => m.AwayTeamId == Id).Sum(m => m.AwayTeamGoals ?? 0);

        [NotMapped]
        public int GoalsAgainst => FinishedMatches.Where(m => m.HomeTeamId == Id).Sum(m => m.AwayTeamGoals ?? 0) + FinishedMatches.Where(m => m.AwayTeamId == Id).Sum(m => m.HomeTeamGoals ?? 0);

        [NotMapped]
        public int GoalDifference => GoalsFor - GoalsAgainst;

        [NotMapped]
        public int Points => (Wins * 3) + Draws;

        // متد کمکی برای دریافت تمام بازی‌های انجام شده
        [NotMapped]
        private IEnumerable<Match> FinishedMatches => HomeMatches.Concat(AwayMatches).Where(m => m.HomeTeamGoals.HasValue && m.AwayTeamGoals.HasValue);
    }
}