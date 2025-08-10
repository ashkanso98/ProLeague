using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProLeague.Application.Interfaces;
using ProLeague.Domain.Entities;

namespace ProLeague.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CommentController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        // GET: Admin/Comment?status=Pending
        public async Task<IActionResult> Index(CommentStatus status = CommentStatus.Pending)
        {
            ViewBag.CurrentStatus = status;
            var comments = await _commentService.GetCommentsByStatusAsync(status);
            return View(comments);
        }

        // GET: Admin/Comment/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // Note: Assumes GetCommentByIdAsync in the service now gets details
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null) return NotFound();
            return View(comment);
        }

        // POST: Admin/Comment/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var result = await _commentService.ApproveCommentAsync(id);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "نظر با موفقیت تأیید شد.";
            }
            return RedirectToAction(nameof(Index), new { status = CommentStatus.Pending });
        }

        // POST: Admin/Comment/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            var result = await _commentService.RejectCommentAsync(id);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "نظر با موفقیت رد شد.";
            }
            return RedirectToAction(nameof(Index), new { status = CommentStatus.Pending });
        }

        // GET: Admin/Comment/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);
            if (comment == null) return NotFound();
            return View(comment);
        }

        // POST: Admin/Comment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Keep track of the status before deleting
            var commentToDelete = await _commentService.GetCommentByIdAsync(id);
            var statusToReturnTo = commentToDelete?.Status ?? CommentStatus.Pending;

            var result = await _commentService.DeleteCommentAsync(id);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "نظر برای همیشه حذف شد.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "خطا در حذف نظر.";
            }

            return RedirectToAction(nameof(Index), new { status = statusToReturnTo });
        }
    }
}