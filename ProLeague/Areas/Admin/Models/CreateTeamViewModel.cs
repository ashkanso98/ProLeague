using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ProLeague.Areas.Admin.Models
{
    // ViewModel برای فرم ایجاد تیم
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
        public IFormFile? LogoFile { get; set; } // برای آپلود فایل
    }

    // ViewModel برای فرم ویرایش تیم
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
        public string? ExistingLogoPath { get; set; } // مسیر لوگوی فعلی برای نمایش

        [Display(Name = "انتخاب لوگوی جدید")]
        public IFormFile? NewLogoFile { get; set; } // برای آپلود فایل جدید

        // آمار تیم
        [Display(Name = "تعداد بازی")]
        [Range(0, int.MaxValue, ErrorMessage = "مقدار نمی‌تواند منفی باشد.")]
        public int Played { get; set; }

        [Display(Name = "برد")]
        [Range(0, int.MaxValue, ErrorMessage = "مقدار نمی‌تواند منفی باشد.")]
        public int Wins { get; set; }

        [Display(Name = "مساوی")]
        [Range(0, int.MaxValue, ErrorMessage = "مقدار نمی‌تواند منفی باشد.")]
        public int Draws { get; set; }

        [Display(Name = "باخت")]
        [Range(0, int.MaxValue, ErrorMessage = "مقدار نمی‌تواند منفی باشد.")]
        public int Losses { get; set; }

        [Display(Name = "گل زده")]
        [Range(0, int.MaxValue, ErrorMessage = "مقدار نمی‌تواند منفی باشد.")]
        public int GoalsFor { get; set; }

        [Display(Name = "گل خورده")]
        [Range(0, int.MaxValue, ErrorMessage = "مقدار نمی‌تواند منفی باشد.")]
        public int GoalsAgainst { get; set; }
    }
}