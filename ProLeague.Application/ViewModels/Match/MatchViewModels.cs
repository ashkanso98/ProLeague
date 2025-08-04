// ProLeague.Application/ViewModels/Match/MatchViewModels.cs
using System;
using System.ComponentModel.DataAnnotations;

namespace ProLeague.Application.ViewModels.Match
{
    public class CreateMatchViewModel
    {
        [Required(ErrorMessage = "انتخاب لیگ اجباری است.")]
        [Display(Name = "لیگ")]
        public int LeagueId { get; set; }

        [Required(ErrorMessage = "انتخاب تیم میزبان اجباری است.")]
        [Display(Name = "تیم میزبان")]
        public int HomeTeamId { get; set; }

        [Required(ErrorMessage = "انتخاب تیم میهمان اجباری است.")]
        [Display(Name = "تیم میهمان")]
        public int AwayTeamId { get; set; }

        [Required(ErrorMessage = "تاریخ و ساعت بازی را مشخص کنید.")]
        [Display(Name = "تاریخ و ساعت بازی")]
        public DateTime MatchDate { get; set; }

        [Required(ErrorMessage = "هفته برگزاری را مشخص کنید.")]
        [Display(Name = "هفته برگزاری")]
        [Range(1, 50, ErrorMessage = "هفته برگزاری باید بین ۱ تا ۵۰ باشد.")]
        public int MatchWeek { get; set; }
    }

    public class UpdateMatchResultViewModel
    {
        [Required]
        public int MatchId { get; set; }

        [Required(ErrorMessage = "تعداد گل میزبان را وارد کنید.")]
        [Display(Name = "گل‌های تیم میزبان")]
        [Range(0, 100, ErrorMessage = "تعداد گل‌ها باید بین ۰ تا ۱۰۰ باشد.")]
        public int HomeTeamGoals { get; set; }

        [Required(ErrorMessage = "تعداد گل میهمان را وارد کنید.")]
        [Display(Name = "گل‌های تیم میهمان")]
        [Range(0, 100, ErrorMessage = "تعداد گل‌ها باید بین ۰ تا ۱۰۰ باشد.")]
        public int AwayTeamGoals { get; set; }
    }
}