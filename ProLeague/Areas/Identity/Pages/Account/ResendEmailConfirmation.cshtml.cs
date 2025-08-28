// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using ProLeague.Domain.Entities;

namespace ProLeague.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ResendEmailConfirmationModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Verification email sent. Please check your email.");
                return Page();
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { userId = userId, code = code },
                protocol: Request.Scheme);
            await _emailSender.SendEmailAsync(
                             Input.Email,
                             "فعال‌سازی حساب کاربری در ProLeague - لطفاً ایمیل خود را تأیید کنید",
                             $@"
    <!DOCTYPE html>
    <html dir='rtl' lang='fa'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <title>تأیید ایمیل</title>
        <style>
            body {{
                font-family: 'Tahoma', 'Arial', sans-serif;
                margin: 0;
                padding: 0;
                background-color: #f5f5f5;
                color: #333;
                line-height: 1.6;
            }}
            .email-container {{
                max-width: 600px;
                margin: 20px auto;
                background: #ffffff;
                border-radius: 12px;
                overflow: hidden;
                box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
                border: 1px solid #e0e0e0;
            }}
            .email-header {{
                background: linear-gradient(135deg, #0066cc 0%, #003366 100%);
                padding: 25px 20px;
                text-align: center;
                color: #ffffff;
            }}
            .email-header h1 {{
                margin: 0;
                font-size: 24px;
                font-weight: bold;
            }}
            .email-body {{
                padding: 30px;
                text-align: center;
            }}
            .email-body p {{
                margin-bottom: 20px;
                font-size: 16px;
                color: #444;
            }}
            .verification-button {{
                display: inline-block;
                background: #0066cc;
                color: #ffffff !important;
                padding: 14px 35px;
                text-decoration: none;
                border-radius: 8px;
                font-weight: bold;
                font-size: 18px;
                margin: 25px 0;
                border: 2px solid #0055aa;
                transition: all 0.3s ease;
                box-shadow: 0 3px 6px rgba(0, 102, 204, 0.2);
            }}
            .verification-button:hover {{
                background: #0055aa;
                transform: translateY(-2px);
                box-shadow: 0 5px 12px rgba(0, 102, 204, 0.3);
            }}
            .email-footer {{
                padding: 20px;
                text-align: center;
                font-size: 12px;
                color: #777;
                background: #f9f9f9;
                border-top: 1px solid #eeeeee;
            }}
            .logo {{
                max-width: 180px;
                margin-bottom: 20px;
            }}
            .highlight {{
                color: #0066cc;
                font-weight: bold;
            }}
            @media only screen and (max-width: 600px) {{
                .email-container {{
                    margin: 10px;
                    border-radius: 8px;
                }}
                .email-body {{
                    padding: 20px;
                }}
                .verification-button {{
                    padding: 12px 25px;
                    font-size: 16px;
                }}
            }}
        </style>
    </head>
    <body>
        <div class='email-container'>
            <div class='email-header'>
                <h1>خوش آمدید به ProLeague</h1>
                <p>پلتفرم تخصصی فوتبال و لیگ‌های حرفه‌ای</p>
            </div>
            <div class='email-body'>
                <p>کاربر گرامی،</p>
                <p>از اینکه در <span class='highlight'>ProLeague</span> ثبت‌نام کردید، سپاسگزاریم. برای تکمیل فرآیند ثبت‌نام و فعال‌سازی حساب کاربری، لطفاً ایمیل خود را از طریق دکمه زیر تأیید کنید:</p>
                
                <a href='{HtmlEncoder.Default.Encode(callbackUrl)}' class='verification-button'>فعال‌سازی حساب کاربری</a>
                
                <p>لینک فوق تنها به مدت <strong>24 ساعت</strong> معتبر است. در صورت انقضا می‌توانید درخواست ارسال مجدد لینک را دهید.</p>
                <p>اگر شما این درخواست را انجام نداده‌اید، لطفاً این ایمیل را نادیده بگیرید.</p>
            </div>
            <div class='email-footer'>
                <p>این ایمیل به صورت خودکار ارسال شده است. لطفاً به آن پاسخ ندهید.</p>
                <p>© {DateTime.Now.Year} ProLeague. تمام حقوق محفوظ است.</p>
            </div>
        </div>
    </body>
    </html>
    ");

            ModelState.AddModelError(string.Empty, "ایمیل تایید ارسال شد. لطفا ایمیل خود را بررسی کنید.");
            return Page();
        }
    }
}
