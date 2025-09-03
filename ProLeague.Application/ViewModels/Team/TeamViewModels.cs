using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
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

        // --- CHANGED ---
        [Required(ErrorMessage = "حداقل انتخاب یک لیگ اجباری است.")]
        [Display(Name = "لیگ‌ها")]
        public List<int> LeagueIds { get; set; } = new List<int>();

        [Display(Name = "لوگوی تیم")]
        public IFormFile? LogoFile { get; set; }
        [Display(Name = "تیم مهم")]
        public bool IsImportant { get; set; }
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

        // --- CHANGED ---
        [Required(ErrorMessage = "حداقل انتخاب یک لیگ اجباری است.")]
        [Display(Name = "لیگ‌ها")]
        public List<int> LeagueIds { get; set; } = new List<int>();

        [Display(Name = "لوگوی فعلی")]
        public string? ExistingLogoPath { get; set; }

        [Display(Name = "انتخاب لوگوی جدید")]
        public IFormFile? NewLogoFile { get; set; }
        [Display(Name = "تیم مهم")]
        public bool IsImportant { get; set; }
    }
}