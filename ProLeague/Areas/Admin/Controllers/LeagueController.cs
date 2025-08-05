using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.League;

namespace ProLeague.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class LeagueController : Controller
    {
        private readonly ILeagueService _leagueService;

        public LeagueController(ILeagueService leagueService)
        {
            _leagueService = leagueService;
        }

        // GET: Admin/League
        public async Task<IActionResult> Index()
        {
            var leagues = await _leagueService.GetAllLeaguesAsync();
            return View(leagues);
        }

        // GET: Admin/League/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var league = await _leagueService.GetLeagueByIdAsync(id);
            if (league == null)
            {
                return NotFound();
            }
            return View(league);
        }

        // GET: Admin/League/Create
        public IActionResult Create()
        {
            return View(new CreateLeagueViewModel());
        }

        // POST: Admin/League/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLeagueViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _leagueService.CreateLeagueAsync(model);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "لیگ با موفقیت ایجاد شد.";
                return RedirectToAction(nameof(Index));
            }

            // If there were errors from the service, add them to the model state
            foreach (var error in result.Errors ?? Enumerable.Empty<string>())
            {
                ModelState.AddModelError(string.Empty, error);
            }
            return View(model);
        }

        // GET: Admin/League/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _leagueService.GetLeagueForEditAsync(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: Admin/League/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditLeagueViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _leagueService.UpdateLeagueAsync(model);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "لیگ با موفقیت به‌روزرسانی شد.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors ?? Enumerable.Empty<string>())
            {
                ModelState.AddModelError(string.Empty, error);
            }
            return View(model);
        }

        // GET: Admin/League/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var league = await _leagueService.GetLeagueByIdAsync(id);
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
            var result = await _leagueService.DeleteLeagueAsync(id);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "لیگ با موفقیت حذف شد.";
            }
            else
            {
                // Optionally, show an error if deletion fails
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "خطا در حذف لیگ.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}