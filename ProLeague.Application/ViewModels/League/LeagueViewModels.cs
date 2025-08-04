// ProLeague.Application/ViewModels/League/LeagueViewModels.cs
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ProLeague.Application.ViewModels.League
{
    public class CreateLeagueViewModel
    {
        [Required(ErrorMessage = "نام لیگ اجباری است.")]
        [StringLength(100)]
        [Display(Name = "نام لیگ")]
        public string Name { get; set; }

        [StringLength(50)]
        [Display(Name = "کشور")]
        public string Country { get; set; }

        [Display(Name = "لوگوی لیگ")]
        public IFormFile? LogoFile { get; set; }
    }

    public class EditLeagueViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام لیگ اجباری است.")]
        [StringLength(100)]
        [Display(Name = "نام لیگ")]
        public string Name { get; set; }

        [StringLength(50)]
        [Display(Name = "کشور")]
        public string Country { get; set; }

        [Display(Name = "لوگوی فعلی")]
        public string? ExistingLogoPath { get; set; }

        [Display(Name = "انتخاب لوگوی جدید")]
        public IFormFile? NewLogoFile { get; set; }
    }
}