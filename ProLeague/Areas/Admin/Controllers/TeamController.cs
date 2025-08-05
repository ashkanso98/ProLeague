using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.Team;

namespace ProLeague.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TeamController : Controller
    {
        private readonly ITeamService _teamService;
        private readonly ILeagueService _leagueService; // Needed for the dropdown list

        public TeamController(ITeamService teamService, ILeagueService leagueService)
        {
            _teamService = teamService;
            _leagueService = leagueService;
        }

        // GET: Admin/Team
        public async Task<IActionResult> Index()
        {
            var teams = await _teamService.GetAllTeamsAsync();
            return View(teams);
        }

        // GET: Admin/Team/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var team = await _teamService.GetTeamDetailsAsync(id);
            if (team == null) return NotFound();
            return View(team);
        }

        // GET: Admin/Team/Create
        public async Task<IActionResult> Create()
        {
            await PopulateLeaguesDropDownList();
            return View(new CreateTeamViewModel());
        }

        // POST: Admin/Team/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTeamViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateLeaguesDropDownList(model.LeagueId);
                return View(model);
            }

            var result = await _teamService.CreateTeamAsync(model);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "تیم با موفقیت ایجاد شد.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Errors?.FirstOrDefault() ?? "An unknown error occurred.");
            await PopulateLeaguesDropDownList(model.LeagueId);
            return View(model);
        }

        // GET: Admin/Team/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _teamService.GetTeamForEditAsync(id);
            if (model == null) return NotFound();

            await PopulateLeaguesDropDownList(model.LeagueId);
            return View(model);
        }

        // POST: Admin/Team/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditTeamViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateLeaguesDropDownList(model.LeagueId);
                return View(model);
            }

            var result = await _teamService.UpdateTeamAsync(model);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "تیم با موفقیت به‌روزرسانی شد.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Errors?.FirstOrDefault() ?? "An unknown error occurred.");
            await PopulateLeaguesDropDownList(model.LeagueId);
            return View(model);
        }

        // GET: Admin/Team/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var team = await _teamService.GetTeamDetailsAsync(id);
            if (team == null) return NotFound();
            return View(team);
        }

        // POST: Admin/Team/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _teamService.DeleteTeamAsync(id);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "تیم با موفقیت حذف شد.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "خطا در حذف تیم.";
            }
            return RedirectToAction(nameof(Index));
        }

        // Helper method to populate the leagues dropdown
        private async Task PopulateLeaguesDropDownList(object? selectedLeague = null)
        {
            var leagues = await _leagueService.GetAllLeaguesAsync();
            ViewBag.LeagueId = new SelectList(leagues.OrderBy(l => l.Name), "Id", "Name", selectedLeague);
        }
    }
}