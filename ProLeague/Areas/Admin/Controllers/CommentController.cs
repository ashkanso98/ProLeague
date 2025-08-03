using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProLeague.Domain.Entities;
using ProLeague.Infrastructure.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ProLeague.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CommentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CommentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Comment
        // نمایش لیست نظرات بر اساس وضعیت
        public async Task<IActionResult> Index(CommentStatus status = CommentStatus.Pending)
        {
            ViewBag.CurrentStatus = status;
            var comments = await _context.NewsComments
                .Where(c => c.Status == status)
                .Include(c => c.User)
                .Include(c => c.News)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();

            return View(comments);
        }

        // GET: Admin/Comment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var comment = await _context.NewsComments
                .Include(c => c.User)
                .Include(c => c.News)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null) return NotFound();
            return View(comment);
        }

        // POST: Admin/Comment/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var comment = await _context.NewsComments.FindAsync(id);
            if (comment != null)
            {
                comment.Status = CommentStatus.Approved;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "نظر با موفقیت تأیید شد.";
            }
            return RedirectToAction(nameof(Index), new { status = CommentStatus.Pending });
        }

        // POST: Admin/Comment/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var comment = await _context.NewsComments.FindAsync(id);
            if (comment != null)
            {
                comment.Status = CommentStatus.Rejected;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "نظر با موفقیت رد شد.";
            }
            return RedirectToAction(nameof(Index), new { status = CommentStatus.Pending });
        }

        // GET: Admin/Comment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var comment = await _context.NewsComments
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (comment == null) return NotFound();
            return View(comment);
        }

        // POST: Admin/Comment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.NewsComments.FindAsync(id);
            if (comment != null)
            {
                _context.NewsComments.Remove(comment);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "نظر برای همیشه حذف شد.";
            }
            return RedirectToAction(nameof(Index), new { status = comment?.Status ?? CommentStatus.Pending });
        }
    }
}