using Microsoft.AspNetCore.Http;
using ProLeague.Domain.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProLeague.Areas.Admin.Models
{
    // ViewModel برای فرم ایجاد خبر
    public class CreateNewsViewModel
    {
        [Required(ErrorMessage = "عنوان خبر اجباری است.")]
        [StringLength(200)]
        [Display(Name = "عنوان خبر")]
        public string Title { get; set; }

        [Required(ErrorMessage = "محتوای خبر اجباری است.")]
        [Display(Name = "محتوای کامل خبر")]
        public string Content { get; set; }

        [Display(Name = "خبر ویژه (نمایش در صفحه اصلی)")]
        public bool IsFeatured { get; set; }

        [Display(Name = "تصویر اصلی خبر")]
        public IFormFile? MainImageFile { get; set; }

        [Display(Name = "گالری تصاویر (اختیاری)")]
        public List<IFormFile>? GalleryFiles { get; set; }

        [Display(Name = "لیگ‌های مرتبط")]
        public List<int>? RelatedLeagueIds { get; set; } = new();

        [Display(Name = "تیم‌های مرتبط")]
        public List<int>? RelatedTeamIds { get; set; } = new();

        [Display(Name = "بازیکنان مرتبط")]
        public List<int>? RelatedPlayerIds { get; set; } = new();
    }

    // ViewModel برای فرم ویرایش خبر
    public class EditNewsViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "عنوان خبر اجباری است.")]
        [StringLength(200)]
        [Display(Name = "عنوان خبر")]
        public string Title { get; set; }

        [Required(ErrorMessage = "محتوای خبر اجباری است.")]
        [Display(Name = "محتوای کامل خبر")]
        public string Content { get; set; }

        [Display(Name = "خبر ویژه (نمایش در صفحه اصلی)")]
        public bool IsFeatured { get; set; }

        [Display(Name = "تصویر اصلی فعلی")]
        public string? ExistingMainImagePath { get; set; }

        [Display(Name = "جایگزینی تصویر اصلی")]
        public IFormFile? NewMainImageFile { get; set; }

        [Display(Name = "گالری تصاویر فعلی")]
        public List<NewsImage> ExistingGalleryImages { get; set; } = new();

        [Display(Name = "افزودن تصاویر جدید به گالری")]
        public List<IFormFile>? NewGalleryFiles { get; set; }

        // --- روابط چند به چند ---
        [Display(Name = "لیگ‌های مرتبط")]
        public List<int>? RelatedLeagueIds { get; set; } = new();

        [Display(Name = "تیم‌های مرتبط")]
        public List<int>? RelatedTeamIds { get; set; } = new();

        [Display(Name = "بازیکنان مرتبط")]
        public List<int>? RelatedPlayerIds { get; set; } = new();
    }
}