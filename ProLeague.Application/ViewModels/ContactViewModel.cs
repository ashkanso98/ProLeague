using System.ComponentModel.DataAnnotations;

namespace ProLeague.Application.ViewModels
{
    public class ContactViewModel
    {
        [Required(ErrorMessage = "لطفاً نام خود را وارد کنید.")]
        [Display(Name = "نام کامل شما")]
        public string Name { get; set; }

        [Required(ErrorMessage = "لطفاً ایمیل خود را وارد کنید.")]
        [EmailAddress(ErrorMessage = "لطفاً یک ایمیل معتبر وارد کنید.")]
        [Display(Name = "آدرس ایمیل")]
        public string Email { get; set; }

        [Required(ErrorMessage = "لطفاً موضوع پیام را مشخص کنید.")]
        [Display(Name = "موضوع")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "متن پیام نمی‌تواند خالی باشد.")]
        [Display(Name = "پیام شما")]
        public string Message { get; set; }
    }
}