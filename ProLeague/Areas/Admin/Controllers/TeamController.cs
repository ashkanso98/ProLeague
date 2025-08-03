using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProLeague.Domain.Entities;
using ProLeague.Infrastructure.Data;
using ProLeague.Areas.Admin.Models; // استفاده از ViewModel ها
using Microsoft.AspNetCore.Hosting; // برای کار با فایل‌ها
using System.IO; // برای کار با فایل سیستم

namespace ProLeague.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TeamController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment; // برای دسترسی به wwwroot

        public TeamController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Admin/Team
        public async Task<IActionResult> Index()
        {
            var teams = await _context.Teams.Include(t => t.League).ToListAsync();
            return View(teams);
        }

        // GET: Admin/Team/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var team = await _context.Teams
                .Include(t => t.League)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null) return NotFound();
            return View(team);
        }

        // GET: Admin/Team/Create
        public IActionResult Create()
        {
            ViewData["LeagueId"] = new SelectList(_context.Leagues.OrderBy(l => l.Name), "Id", "Name");
            return View(new CreateTeamViewModel());
        }

        // POST: Admin/Team/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTeamViewModel model)
        {
            if (ModelState.IsValid)
            {
                string? logoPath = await UploadFile(model.LogoFile);

                var team = new Team
                {
                    Name = model.Name,
                    Stadium = model.Stadium,
                    LeagueId = model.LeagueId,
                    ImagePath = logoPath,
                    // آمار اولیه به صورت خودکار صفر در نظر گرفته می‌شود
                };

                _context.Add(team);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "تیم با موفقیت ایجاد شد.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["LeagueId"] = new SelectList(_context.Leagues.OrderBy(l => l.Name), "Id", "Name", model.LeagueId);
            return View(model);
        }

        // GET: Admin/Team/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var team = await _context.Teams.FindAsync(id);
            if (team == null) return NotFound();

            var model = new EditTeamViewModel
            {
                Id = team.Id,
                Name = team.Name,
                Stadium = team.Stadium,
                LeagueId = team.LeagueId,
                ExistingLogoPath = team.ImagePath,
                Played = team.Played,
                Wins = team.Wins,
                Draws = team.Draws,
                Losses = team.Losses,
                GoalsFor = team.GoalsFor,
                GoalsAgainst = team.GoalsAgainst
            };

            ViewData["LeagueId"] = new SelectList(_context.Leagues.OrderBy(l => l.Name), "Id", "Name", team.LeagueId);
            return View(model);
        }

        // POST: Admin/Team/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditTeamViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var teamToUpdate = await _context.Teams.FindAsync(id);
                if (teamToUpdate == null) return NotFound();

                if (model.NewLogoFile != null)
                {
                    // حذف فایل قدیمی در صورت وجود
                    DeleteFile(teamToUpdate.ImagePath);
                    // آپلود فایل جدید
                    teamToUpdate.ImagePath = await UploadFile(model.NewLogoFile);
                }

                // به‌روزرسانی سایر اطلاعات
                teamToUpdate.Name = model.Name;
                teamToUpdate.Stadium = model.Stadium;
                teamToUpdate.LeagueId = model.LeagueId;
                teamToUpdate.Played = model.Played;
                teamToUpdate.Wins = model.Wins;
                teamToUpdate.Draws = model.Draws;
                teamToUpdate.Losses = model.Losses;
                teamToUpdate.GoalsFor = model.GoalsFor;
                teamToUpdate.GoalsAgainst = model.GoalsAgainst;

                try
                {
                    _context.Update(teamToUpdate);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "تیم با موفقیت به‌روزرسانی شد.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(model.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["LeagueId"] = new SelectList(_context.Leagues.OrderBy(l => l.Name), "Id", "Name", model.LeagueId);
            return View(model);
        }

        // GET: Admin/Team/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var team = await _context.Teams
                .Include(t => t.League)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null) return NotFound();
            return View(team);
        }

        // POST: Admin/Team/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team != null)
            {
                DeleteFile(team.ImagePath); // حذف لوگوی تیم از سرور
                _context.Teams.Remove(team);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "تیم با موفقیت حذف شد.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.Id == id);
        }

        // متد کمکی برای آپلود فایل
        private async Task<string?> UploadFile(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;

            string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "teams");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/images/teams/{uniqueFileName}"; // ذخیره مسیر نسبی
        }

        // متد کمکی برای حذف فایل
        private void DeleteFile(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;

            string filePath = Path.Combine(_environment.WebRootPath, relativePath.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}