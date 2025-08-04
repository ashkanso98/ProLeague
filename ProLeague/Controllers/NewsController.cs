// ProLeague/Controllers/NewsController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProLeague.Application.Interfaces;
using ProLeague.Domain.Entities;

namespace ProLeague.Controllers
{
    public class NewsController : Controller
    {
        private readonly INewsService _newsService;
        private readonly ICommentService _commentService; // برای ثبت کامنت
        private readonly UserManager<ApplicationUser> _userManager;

        public NewsController(INewsService newsService, ICommentService commentService, UserManager<ApplicationUser> userManager)
        {
            _newsService = newsService;
            _commentService = commentService;
            _userManager = userManager;
        }

        // GET: News/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var news = await _newsService.GetNewsDetailsAsync(id);
            if (news == null)
            {
                return NotFound();
            }
            // مهم: فیلتر کردن کامنت‌ها برای نمایش فقط موارد تایید شده
            news.Comments = news.Comments.Where(c => c.Status == CommentStatus.Approved).ToList();
            return View(news);
        }

        // POST: News/AddComment
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int newsId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                TempData["ErrorMessage"] = "متن نظر نمی‌تواند خالی باشد.";
                return RedirectToAction(nameof(Details), new { id = newsId });
            }

            var userId = _userManager.GetUserId(User);
            await _commentService.SubmitCommentAsync(newsId, content, userId); // این متد باید در سرویس پیاده‌سازی شود

            TempData["SuccessMessage"] = "نظر شما ثبت شد و پس از تایید نمایش داده خواهد شد.";
            return RedirectToAction(nameof(Details), new { id = newsId });
        }
    }
}