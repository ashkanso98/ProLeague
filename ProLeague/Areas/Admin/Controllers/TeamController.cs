using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.Team;
using System.Linq;
using System.Threading.Tasks;

namespace ProLeague.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TeamController : Controller
    {
        private readonly ITeamService _teamService;
        private readonly ILeagueService _leagueService;

        public TeamController(ITeamService teamService, ILeagueService leagueService)
        {
            _teamService = teamService;
            _leagueService = leagueService;
        }

        // GET: Admin/Team
        public async Task<IActionResult> Index()
        {
            // This method needs to be updated in the repository to include LeagueEntries.League
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
                await PopulateLeaguesDropDownList(model.LeagueIds);
                return View(model);
            }

            var result = await _teamService.CreateTeamAsync(model);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Team created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Errors?.FirstOrDefault() ?? "An unknown error occurred.");
            await PopulateLeaguesDropDownList(model.LeagueIds);
            return View(model);
        }

        // GET: Admin/Team/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _teamService.GetTeamForEditAsync(id);
            if (model == null) return NotFound();

            await PopulateLeaguesDropDownList(model.LeagueIds);
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
                await PopulateLeaguesDropDownList(model.LeagueIds);
                return View(model);
            }

            var result = await _teamService.UpdateTeamAsync(model);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Team updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Errors?.FirstOrDefault() ?? "An unknown error occurred.");
            await PopulateLeaguesDropDownList(model.LeagueIds);
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
                TempData["SuccessMessage"] = "Team deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "Error deleting team.";
            }
            return RedirectToAction(nameof(Index));
        }

        // Helper method to populate the leagues dropdown
        private async Task PopulateLeaguesDropDownList(object? selectedLeagues = null)
        {
            var leagues = await _leagueService.GetAllLeaguesAsync();
            // Use MultiSelectList for many-to-many relationships
            ViewBag.Leagues = new MultiSelectList(leagues.OrderBy(l => l.Name), "Id", "Name", selectedLeagues as System.Collections.IEnumerable);
        }
    }
}