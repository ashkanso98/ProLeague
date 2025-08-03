using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProLeague.Areas.Admin.Models;
using ProLeague.Domain.Entities;
using ProLeague.Infrastructure.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ProLeague.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class NewsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public NewsController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IActionResult> Index()
        {
            var news = await _context.News.OrderByDescending(n => n.PublishedDate).ToListAsync();
            return View(news);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var news = await _context.News
                .Include(n => n.Images)
                .Include(n => n.RelatedLeagues)
                .Include(n => n.RelatedTeams)
                .Include(n => n.RelatedPlayers)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (news == null) return NotFound();
            return View(news);
        }

        public IActionResult Create()
        {
            PopulateRelatedDataForView();
            return View(new CreateNewsViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateNewsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var news = new News
                {
                    Title = model.Title,
                    Content = model.Content,
                    IsFeatured = model.IsFeatured,
                    PublishedDate = DateTime.UtcNow,
                    MainImagePath = await UploadFile(model.MainImageFile, "news/main")
                };

                // افزودن تصاویر گالری
                if (model.GalleryFiles != null)
                {
                    foreach (var file in model.GalleryFiles)
                    {
                        news.Images.Add(new NewsImage { ImagePath = await UploadFile(file, "news/gallery") });
                    }
                }

                // افزودن روابط
                await UpdateRelatedEntities(news, model.RelatedLeagueIds, model.RelatedTeamIds, model.RelatedPlayerIds);

                _context.Add(news);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "خبر با موفقیت ایجاد شد.";
                return RedirectToAction(nameof(Index));
            }

            PopulateRelatedDataForView();
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var news = await _context.News.Include(n => n.Images)
                        .Include(n => n.RelatedLeagues)
                        .Include(n => n.RelatedTeams)
                        .Include(n => n.RelatedPlayers)
                        .FirstOrDefaultAsync(n => n.Id == id);
            if (news == null) return NotFound();

            var model = new EditNewsViewModel
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content,
                IsFeatured = news.IsFeatured,
                ExistingMainImagePath = news.MainImagePath,
                ExistingGalleryImages = news.Images.ToList(),
                RelatedLeagueIds = news.RelatedLeagues.Select(l => l.Id).ToList(),
                RelatedTeamIds = news.RelatedTeams.Select(t => t.Id).ToList(),
                RelatedPlayerIds = news.RelatedPlayers.Select(p => p.Id).ToList(),
            };

            PopulateRelatedDataForView(model.RelatedLeagueIds, model.RelatedTeamIds, model.RelatedPlayerIds);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditNewsViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var newsToUpdate = await _context.News.Include(n => n.Images)
                    .Include(n => n.RelatedLeagues)
                    .Include(n => n.RelatedTeams)
                    .Include(n => n.RelatedPlayers)
                    .FirstOrDefaultAsync(n => n.Id == id);
                if (newsToUpdate == null) return NotFound();

                // بروزرسانی فیلدهای اصلی
                newsToUpdate.Title = model.Title;
                newsToUpdate.Content = model.Content;
                newsToUpdate.IsFeatured = model.IsFeatured;

                // بروزرسانی تصویر اصلی
                if (model.NewMainImageFile != null)
                {
                    DeleteFile(newsToUpdate.MainImagePath);
                    newsToUpdate.MainImagePath = await UploadFile(model.NewMainImageFile, "news/main");
                }

                // افزودن تصاویر جدید به گالری
                if (model.NewGalleryFiles != null)
                {
                    foreach (var file in model.NewGalleryFiles)
                    {
                        newsToUpdate.Images.Add(new NewsImage { ImagePath = await UploadFile(file, "news/gallery") });
                    }
                }

                // بروزرسانی روابط چند-به-چند
                await UpdateRelatedEntities(newsToUpdate, model.RelatedLeagueIds, model.RelatedTeamIds, model.RelatedPlayerIds);

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "خبر با موفقیت به‌روزرسانی شد.";
                return RedirectToAction(nameof(Index));
            }

            PopulateRelatedDataForView(model.RelatedLeagueIds, model.RelatedTeamIds, model.RelatedPlayerIds);
            return View(model);
        }

        // POST: برای حذف یک تصویر از گالری در صفحه Edit
        [HttpPost]
        public async Task<IActionResult> DeleteGalleryImage(int imageId)
        {
            var image = await _context.NewsImages.FindAsync(imageId);
            if (image != null)
            {
                DeleteFile(image.ImagePath);
                _context.NewsImages.Remove(image);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var news = await _context.News.FirstOrDefaultAsync(m => m.Id == id);
            if (news == null) return NotFound();
            return View(news);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var news = await _context.News.Include(n => n.Images).FirstOrDefaultAsync(n => n.Id == id);
            if (news != null)
            {
                // حذف تمام تصاویر
                DeleteFile(news.MainImagePath);
                foreach (var image in news.Images)
                {
                    DeleteFile(image.ImagePath);
                }

                _context.News.Remove(news);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "خبر با موفقیت حذف شد.";
            }
            return RedirectToAction(nameof(Index));
        }

        // --- متدهای کمکی ---
        private async Task<string> UploadFile(IFormFile file, string subfolder)
        {
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", subfolder);
            Directory.CreateDirectory(uploadsFolder);
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return $"/images/{subfolder}/{uniqueFileName}";
        }

        private void DeleteFile(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;
            string filePath = Path.Combine(_environment.WebRootPath, relativePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }

        private void PopulateRelatedDataForView(object? leagues = null, object? teams = null, object? players = null)
        {
            ViewData["Leagues"] = new MultiSelectList(_context.Leagues.OrderBy(l => l.Name), "Id", "Name", (List<int>)leagues);
            ViewData["Teams"] = new MultiSelectList(_context.Teams.OrderBy(t => t.Name), "Id", "Name", (List<int>)teams);
            ViewData["Players"] = new MultiSelectList(_context.Players.OrderBy(p => p.Name), "Id", "Name", (List<int>)players);
        }

        private async Task UpdateRelatedEntities(News news, List<int>? leagueIds, List<int>? teamIds, List<int>? playerIds)
        {
            news.RelatedLeagues.Clear();
            if (leagueIds != null)
            {
                var selectedLeagues = await _context.Leagues.Where(l => leagueIds.Contains(l.Id)).ToListAsync();
                foreach (var league in selectedLeagues) news.RelatedLeagues.Add(league);
            }

            news.RelatedTeams.Clear();
            if (teamIds != null)
            {
                var selectedTeams = await _context.Teams.Where(t => teamIds.Contains(t.Id)).ToListAsync();
                foreach (var team in selectedTeams) news.RelatedTeams.Add(team);
            }

            news.RelatedPlayers.Clear();
            if (playerIds != null)
            {
                var selectedPlayers = await _context.Players.Where(p => playerIds.Contains(p.Id)).ToListAsync();
                foreach (var player in selectedPlayers) news.RelatedPlayers.Add(player);
            }
        }
    }
}