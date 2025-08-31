using System.ComponentModel.DataAnnotations;

namespace ProLeague.Application.ViewModels.Admin
{
    public class CreatePointDeductionViewModel
    {
        [Required]
        public int LeagueId { get; set; }

        [Required(ErrorMessage = "Please select a team.")]
        [Display(Name = "Team")]
        public int TeamId { get; set; }

        [Required(ErrorMessage = "Please enter the number of points to deduct.")]
        [Display(Name = "Points to Deduct")]
        [Range(1, 100, ErrorMessage = "Points must be between 1 and 100.")]
        public int Points { get; set; }

        [Required(ErrorMessage = "Please provide a reason.")]
        [Display(Name = "Reason")]
        [StringLength(200)]
        public string Reason { get; set; }
    }
}