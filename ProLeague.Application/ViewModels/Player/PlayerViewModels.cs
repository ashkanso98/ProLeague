// ProLeague.Application/ViewModels/Player/PlayerViewModels.cs
using Microsoft.AspNetCore.Http;
using ProLeague.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace ProLeague.Application.ViewModels.Player
{
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

        [Display(Name = "تعداد گل")]
        [Range(0, int.MaxValue, ErrorMessage = "مقدار نمی‌تواند منفی باشد.")]
        public int Goals { get; set; }

        [Display(Name = "پاس گل")]
        [Range(0, int.MaxValue, ErrorMessage = "مقدار نمی‌تواند منفی باشد.")]
        public int Assists { get; set; }
    }
}