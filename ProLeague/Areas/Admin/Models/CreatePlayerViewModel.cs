using Microsoft.AspNetCore.Http;
using ProLeague.Domain.Entities; // برای دسترسی به enum پست بازیکن
using System.ComponentModel.DataAnnotations;

namespace ProLeague.Areas.Admin.Models
{
    // ViewModel برای فرم ایجاد بازیکن
    public class CreatePlayerViewModel
    {
        [Required(ErrorMessage = "نام بازیکن اجباری است.")]
        [StringLength(100)]
        [Display(Name = "نام کامل بازیکن")]
        public string Name { get; set; }

        [Required(ErrorMessage = "پست بازیکن را مشخص کنید.")]
        [Display(Name = "پست تخصصی")]
        public Position Position { get; set; }

        [Required(ErrorMessage = "تیم بازیکن را مشخص کنید.")]
        [Display(Name = "تیم")]
        public int TeamId { get; set; }

        [Display(Name = "عکس بازیکن")]
        public IFormFile? PhotoFile { get; set; }
    }

    // ViewModel برای فرم ویرایش بازیکن
    public class EditPlayerViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "نام بازیکن اجباری است.")]
        [StringLength(100)]
        [Display(Name = "نام کامل بازیکن")]
        public string Name { get; set; }

        [Required(ErrorMessage = "پست بازیکن را مشخص کنید.")]
        [Display(Name = "پست تخصصی")]
        public Position Position { get; set; }

        [Required(ErrorMessage = "تیم بازیکن را مشخص کنید.")]
        [Display(Name = "تیم")]
        public int TeamId { get; set; }

        [Display(Name = "عکس فعلی")]
        public string? ExistingPhotoPath { get; set; }

        [Display(Name = "انتخاب عکس جدید")]
        public IFormFile? NewPhotoFile { get; set; }

        // آمار
        [Display(Name = "تعداد گل")]
        [Range(0, int.MaxValue, ErrorMessage = "مقدار نمی‌تواند منفی باشد.")]
        public int Goals { get; set; }

        [Display(Name = "پاس گل")]
        [Range(0, int.MaxValue, ErrorMessage = "مقدار نمی‌تواند منفی باشد.")]
        public int Assists { get; set; }
    }
}