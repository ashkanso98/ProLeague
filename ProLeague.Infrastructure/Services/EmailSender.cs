using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace ProLeague.Infrastructure.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // این متد در محیط توسعه هیچ کاری انجام نمی‌دهد.
            // در محیط واقعی، اینجا کد ارسال ایمیل با یک سرویس واقعی قرار می‌گیرد.
            return Task.CompletedTask;
        }
    }
}