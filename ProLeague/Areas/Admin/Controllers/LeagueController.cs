using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProLeague.Domain.Entities;
using ProLeague.Infrastructure.Data;
using ProLeague.Areas.Admin.Models; // برای ViewModel ها
using Microsoft.AspNetCore.Http; // برای IFormFile
using System.IO; // برای کار با فایل سیستم

namespace ProLeague.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class LeagueController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment; // برای دسترسی به مسیر wwwroot

        public LeagueController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        // ... سایر متدها ...

        // GET: Admin/League/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/League/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLeagueViewModel model)
        {
            if (ModelState.IsValid)
            {
                string? logoPath = null;
                if (model.LogoFile != null && model.LogoFile.Length > 0)
                {
                    // 1. تعیین مسیر ذخیره‌سازی
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "league");
                    // 2. اطمینان از وجود پوشه
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    // 3. ایجاد نام فایل منحصر به فرد
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.LogoFile.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // 4. ذخیره فایل در سرور
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.LogoFile.CopyToAsync(fileStream);
                    }

                    // 5. ذخیره مسیر نسبی در دیتابیس
                    logoPath = $"/images/league/{uniqueFileName}";
                }

                var league = new League
                {
                    Name = model.Name,
                    Country = model.Country,
                    ImagePath = logoPath // مسیر نسبی
                };

                _context.Add(league);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "لیگ با موفقیت ایجاد شد.";
                //return RedirectToAction(nameof(Index));
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Admin/League/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var league = await _context.Leagues.FindAsync(id);
            if (league == null)
            {
                return NotFound();
            }

            var model = new EditLeagueViewModel
            {
                Id = league.Id,
                Name = league.Name,
                Country = league.Country,
                ExistingLogoPath = league.ImagePath
            };

            return View(model);
        }

        // POST: Admin/League/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditLeagueViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var league = await _context.Leagues.FindAsync(id);
                    if (league == null)
                    {
                        return NotFound();
                    }

                    league.Name = model.Name;
                    league.Country = model.Country;

                    // بررسی آپلود فایل جدید
                    if (model.NewLogoFile != null && model.NewLogoFile.Length > 0)
                    {
                        // 1. حذف فایل قدیمی (اختیاری)
                        if (!string.IsNullOrEmpty(league.ImagePath))
                        {
                            string oldFilePath = Path.Combine(_environment.WebRootPath, league.ImagePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // 2. ذخیره فایل جدید
                        string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "league");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }
                        string uniqueFileName = Guid.NewGuid().ToString() + "_" + model.NewLogoFile.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.NewLogoFile.CopyToAsync(fileStream);
                        }

                        league.ImagePath = $"/images/league/{uniqueFileName}";
                    }

                    _context.Update(league);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "لیگ با موفقیت به‌روزرسانی شد.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeagueExists(model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
        // GET: Admin/League
        public async Task<IActionResult> Index()
        {
            // تمام لیگ‌ها را از دیتابیس دریافت کرده و به ویو ارسال می‌کند
            var leagues = await _context.Leagues.ToListAsync();
            return View(leagues);
        }

        // GET: Admin/League/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // لیگ مورد نظر را بر اساس شناسه پیدا می‌کند
            var league = await _context.Leagues
                .FirstOrDefaultAsync(m => m.Id == id);

            if (league == null)
            {
                return NotFound();
            }

            return View(league);
        }

        // GET: Admin/League/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // لیگ مورد نظر را برای نمایش در صفحه تایید حذف، پیدا می‌کند
            var league = await _context.Leagues
                .FirstOrDefaultAsync(m => m.Id == id);
            if (league == null)
            {
                return NotFound();
            }

            return View(league);
        }

        // POST: Admin/League/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var league = await _context.Leagues.FindAsync(id);
            if (league == null)
            {
                // اگر لیگ قبلا حذف شده باشد، کاربر را به صفحه اصلی باز می‌گرداند
                return RedirectToAction(nameof(Index));
            }

            // 1. حذف فایل لوگوی قدیمی از سرور
            if (!string.IsNullOrEmpty(league.ImagePath))
            {
                // مسیر کامل فایل فیزیکی را می‌سازد
                string oldFilePath = Path.Combine(_environment.WebRootPath, league.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
            }

            // 2. حذف رکورد از دیتابیس
            _context.Leagues.Remove(league);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "لیگ با موفقیت حذف شد.";
            return RedirectToAction(nameof(Index));
        }
        private bool LeagueExists(int id)
        {
            return _context.Leagues.Any(e => e.Id == id);
        }
    }
}