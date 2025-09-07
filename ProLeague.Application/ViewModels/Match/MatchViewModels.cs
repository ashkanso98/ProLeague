using System.ComponentModel.DataAnnotations;

namespace ProLeague.Application.ViewModels.Match
{
    public class CreateMatchViewModel
    {
        [Required(ErrorMessage = "League selection is required.")]
        [Display(Name = "League")]
        public int LeagueId { get; set; }

        [Required(ErrorMessage = "Home team selection is required.")]
        [Display(Name = "Home Team")]
        public int HomeTeamId { get; set; }

        [Required(ErrorMessage = "Away team selection is required.")]
        [Display(Name = "Away Team")]
        public int AwayTeamId { get; set; }

        [Required(ErrorMessage = "Match date is required.")]
        [Display(Name = "Match Date & Time")]
        public DateTime MatchDate { get; set; }

        [Required(ErrorMessage = "Match week is required.")]
        [Display(Name = "Match Week")]
        [Range(1, 50, ErrorMessage = "Match week must be between 1 and 50.")]
        public int MatchWeek { get; set; }
        //[Required(ErrorMessage = "Home team goals are required.")]
        [Display(Name = "Home Team Goals")]
        [Range(0, 100, ErrorMessage = "Goals must be between 0 and 100.")]
        public int? HomeTeamGoals { get; set; }

        //[Required(ErrorMessage = "Away team goals are required.")]
        [Display(Name = "Away Team Goals")]
        [Range(0, 100, ErrorMessage = "Goals must be between 0 and 100.")]
        public int? AwayTeamGoals { get; set; }
        [Required]
        [Display(Name = "Season")]
        public string Season { get; set; }
    }

    public class UpdateMatchResultViewModel
    {
        [Required]
        public int MatchId { get; set; }

        //[Required(ErrorMessage = "Home team goals are required.")]
        [Display(Name = "Home Team Goals")]
        [Range(0, 100, ErrorMessage = "Goals must be between 0 and 100.")]
        public int? HomeTeamGoals { get; set; }

        //[Required(ErrorMessage = "Away team goals are required.")]
        [Display(Name = "Away Team Goals")]
        [Range(0, 100, ErrorMessage = "Goals must be between 0 and 100.")]
        public int? AwayTeamGoals { get; set; }

    }
}