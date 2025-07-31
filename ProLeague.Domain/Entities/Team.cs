// ProLeague.Domain/Entities/Team.cs
using System.ComponentModel.DataAnnotations;

namespace ProLeague.Domain.Entities
{
    public class Team
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string? Stadium { get; set; }
        public string? ImagePath { get; set; } // Path to the team logo

        public int LeagueId { get; set; }
        public League League { get; set; } = null!; // Navigation property

        public ICollection<Player> Players { get; set; } = new List<Player>();
        public ICollection<News> News { get; set; } = new List<News>();
        public int FanCount { get; set; } // Number of users who favorited this team

        // فیلدهای مورد نیاز برای جدول لیگ
        public int Played { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Losses { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference => GoalsFor - GoalsAgainst; // محاسبه خودکار
        public int Points => (Wins * 3) + Draws; // محاسبه خودکار
    }
}