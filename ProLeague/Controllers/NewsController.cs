// FootballNews.Web/Controllers/NewsController.cs
using ProLeague.Infrastructure.Data;
using ProLeague.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProLeague.Controllers
{
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NewsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: News/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var news = await _context.News
                .Include(n => n.Images)
                .Include(n => n.Comments)
                    .ThenInclude(c => c.User) // Load user info for comments
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null)
            {
                return NotFound();
            }

            return View(news);
        }

        // POST: News/AddComment
        [HttpPost]
        [Authorize] // Only logged-in users can comment
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddComment(int newsId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                // Handle empty comment, maybe add model state error
                return RedirectToAction(nameof(Details), new { id = newsId });
            }

            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            var comment = new NewsComment
            {
                NewsId = newsId,
                Content = content,
                UserId = userId,
                User = user
            };

            _context.NewsComments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { id = newsId });
        }

        // GET: News/DeleteComment/5 (Admin only)
        [Authorize(Roles = "Admin")] // Requires Admin role
        public async Task<IActionResult> DeleteComment(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.NewsComments
                .Include(c => c.News)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null)
            {
                return NotFound();
            }
            return View(comment); // Confirm deletion view
        }

        // POST: News/DeleteComment/5
        [HttpPost, ActionName("DeleteComment")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCommentConfirmed(int id)
        {
            var comment = await _context.NewsComments.FindAsync(id);
            if (comment != null)
            {
                _context.NewsComments.Remove(comment);
                await _context.SaveChangesAsync();
            }
            // Redirect back to the news article
            return RedirectToAction(nameof(Details), new { id = comment?.NewsId });
        }
    }
}