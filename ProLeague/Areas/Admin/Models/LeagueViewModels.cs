using Microsoft.AspNetCore.Http; // برای IFormFile
using System.ComponentModel.DataAnnotations;

namespace ProLeague.Areas.Admin.Models
{
    public class CreateLeagueViewModel
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Country { get; set; }

        [Display(Name = "لوگوی لیگ")]
        public IFormFile? LogoFile { get; set; } // فایل آپلودی
    }

    public class EditLeagueViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(50)]
        public string Country { get; set; }

        [Display(Name = "لوگوی فعلی")]
        public string? ExistingLogoPath { get; set; } // برای نمایش تصویر فعلی

        [Display(Name = "لوگوی جدید")]
        public IFormFile? NewLogoFile { get; set; } // فایل آپلودی جدید
    }
}