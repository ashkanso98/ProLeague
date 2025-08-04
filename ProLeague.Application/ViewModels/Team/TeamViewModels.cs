// ProLeague.Application/ViewModels/Team/TeamViewModels.cs
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ProLeague.Application.ViewModels.Team
{
    public class CreateTeamViewModel
    {
        [Required(ErrorMessage = "نام تیم اجباری است.")]
        [StringLength(100)]
        [Display(Name = "نام تیم")]
        public string Name { get; set; }

        [StringLength(100)]
        [Display(Name = "ورزشگاه")]
        public string? Stadium { get; set; }

        [Required(ErrorMessage = "انتخاب لیگ اجباری است.")]
        [Display(Name = "لیگ")]
        public int LeagueId { get; set; }

        [Display(Name = "لوگوی تیم")]
        public IFormFile? LogoFile { get; set; }
    }

    public class EditTeamViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام تیم اجباری است.")]
        [StringLength(100)]
        [Display(Name = "نام تیم")]
        public string Name { get; set; }

        [StringLength(100)]
        [Display(Name = "ورزشگاه")]
        public string? Stadium { get; set; }

        [Required(ErrorMessage = "انتخاب لیگ اجباری است.")]
        [Display(Name = "لیگ")]
        public int LeagueId { get; set; }

        [Display(Name = "لوگوی فعلی")]
        public string? ExistingLogoPath { get; set; }

        [Display(Name = "انتخاب لوگوی جدید")]
        public IFormFile? NewLogoFile { get; set; }

        // توجه: فیلدهای آمار (برد، باخت و...) حذف شده‌اند چون دیگر به صورت دستی ویرایش نمی‌شوند.
    }
}