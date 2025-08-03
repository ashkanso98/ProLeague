using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProLeague.Areas.Admin.Models;
using ProLeague.Domain.Entities;
using ProLeague.Infrastructure.Data;
using System.IO;
using System.Threading.Tasks;

namespace ProLeague.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PlayerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public PlayerController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // GET: Admin/Player
        public async Task<IActionResult> Index()
        {
            var players = await _context.Players.Include(p => p.Team).ToListAsync();
            return View(players);
        }

        // GET: Admin/Player/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var player = await _context.Players
                .Include(p => p.Team)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (player == null) return NotFound();
            return View(player);
        }

        // GET: Admin/Player/Create
        public IActionResult Create()
        {
            PopulateTeamsDropDownList();
            return View(new CreatePlayerViewModel());
        }

        // POST: Admin/Player/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePlayerViewModel model)
        {
            if (ModelState.IsValid)
            {
                string? photoPath = await UploadFile(model.PhotoFile);

                var player = new Player
                {
                    Name = model.Name,
                    Position = model.Position,
                    TeamId = model.TeamId,
                    ImagePath = photoPath,
                    Goals = 0, // مقدار اولیه
                    Assists = 0 // مقدار اولیه
                };

                _context.Add(player);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "بازیکن با موفقیت ایجاد شد.";
                return RedirectToAction(nameof(Index));
            }
            PopulateTeamsDropDownList(model.TeamId);
            return View(model);
        }

        // GET: Admin/Player/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var player = await _context.Players.FindAsync(id);
            if (player == null) return NotFound();

            var model = new EditPlayerViewModel
            {
                Id = player.Id,
                Name = player.Name,
                Position = player.Position,
                TeamId = player.TeamId,
                Goals = player.Goals,
                Assists = player.Assists,
                ExistingPhotoPath = player.ImagePath
            };

            PopulateTeamsDropDownList(player.TeamId);
            return View(model);
        }

        // POST: Admin/Player/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditPlayerViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var playerToUpdate = await _context.Players.FindAsync(id);
                if (playerToUpdate == null) return NotFound();

                if (model.NewPhotoFile != null)
                {
                    DeleteFile(playerToUpdate.ImagePath);
                    playerToUpdate.ImagePath = await UploadFile(model.NewPhotoFile);
                }

                playerToUpdate.Name = model.Name;
                playerToUpdate.Position = model.Position;
                playerToUpdate.TeamId = model.TeamId;
                playerToUpdate.Goals = model.Goals;
                playerToUpdate.Assists = model.Assists;

                try
                {
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "اطلاعات بازیکن با موفقیت به‌روزرسانی شد.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlayerExists(model.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateTeamsDropDownList(model.TeamId);
            return View(model);
        }

        // GET: Admin/Player/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var player = await _context.Players.Include(p => p.Team).FirstOrDefaultAsync(m => m.Id == id);
            if (player == null) return NotFound();
            return View(player);
        }

        // POST: Admin/Player/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var player = await _context.Players.FindAsync(id);
            if (player != null)
            {
                DeleteFile(player.ImagePath);
                _context.Players.Remove(player);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "بازیکن با موفقیت حذف شد.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PlayerExists(int id) => _context.Players.Any(e => e.Id == id);

        private void PopulateTeamsDropDownList(object? selectedTeam = null)
        {
            var teamsQuery = _context.Teams.OrderBy(t => t.Name);
            ViewData["TeamId"] = new SelectList(teamsQuery, "Id", "Name", selectedTeam);
        }

        private async Task<string?> UploadFile(IFormFile? file)
        {
            if (file == null || file.Length == 0) return null;
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "players");
            Directory.CreateDirectory(uploadsFolder); // اگر پوشه وجود نداشته باشد آن را می‌سازد
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return $"/images/players/{uniqueFileName}";
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
    }
}