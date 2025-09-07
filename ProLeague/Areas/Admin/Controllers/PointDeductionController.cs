using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.Admin;

namespace ProLeague.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PointDeductionController : Controller
    {
        private readonly IPointDeductionService _deductionService;
        private readonly ILeagueService _leagueService;
        private readonly ITeamService _teamService;
        private readonly IConfiguration _configuration;

        public PointDeductionController(IPointDeductionService deductionService, ILeagueService leagueService, ITeamService teamService, IConfiguration configuration)
        {
            _deductionService = deductionService;
            _leagueService = leagueService;
            _teamService = teamService;
            _configuration = configuration;
        }

        // GET: Admin/PointDeduction?leagueId=5
        public async Task<IActionResult> Index(int leagueId)
        {
            var league = await _leagueService.GetLeagueByIdAsync(leagueId);
            if (league == null) return NotFound();

            ViewBag.League = league;
            var deductions = await _deductionService.GetDeductionsByLeagueAsync(leagueId);
            return View(deductions);
        }

        // GET: Admin/PointDeduction/Create?leagueId=5
        //public async Task<IActionResult> Create(int leagueId)
        //{
        //    var league = await _leagueService.GetLeagueByIdAsync(leagueId);
        //    if (league == null) return NotFound();

        //    var teamsInLeague = await _teamService.GetTeamsByLeagueIdAsync(leagueId);
        //    ViewBag.Teams = new SelectList(teamsInLeague, "Id", "Name");
        //    ViewBag.LeagueName = league.Name;

        //    var model = new CreatePointDeductionViewModel { LeagueId = leagueId };
        //    return View(model);
        //}

        // POST: Admin/PointDeduction/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePointDeductionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _deductionService.CreateDeductionAsync(model);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Point deduction was successfully created.";
                    return RedirectToAction(nameof(Index), new { leagueId = model.LeagueId });
                }
                ModelState.AddModelError(string.Empty, result.Errors.First());
            }

            var teamsInLeague = await _teamService.GetTeamsByLeagueIdAsync(model.LeagueId);
            ViewBag.Teams = new SelectList(teamsInLeague, "Id", "Name", model.TeamId);
            var league = await _leagueService.GetLeagueByIdAsync(model.LeagueId);
            ViewBag.LeagueName = league?.Name;
            return View(model);
        }

        // GET: Admin/PointDeduction/Delete/1
        public async Task<IActionResult> Delete(int id)
        {
            var deduction = await _deductionService.GetDeductionByIdAsync(id);
            if (deduction == null) return NotFound();
            return View(deduction);
        }

        // POST: Admin/PointDeduction/Delete/1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var deduction = await _deductionService.GetDeductionByIdAsync(id);
            if (deduction == null) return NotFound();

            await _deductionService.DeleteDeductionAsync(id);
            TempData["SuccessMessage"] = "Point deduction was successfully deleted.";
            return RedirectToAction(nameof(Index), new { leagueId = deduction.LeagueId });
        }

        //**

        public async Task<IActionResult> Create(int leagueId)
        {
            var league = await _leagueService.GetLeagueByIdAsync(leagueId);
            if (league == null) return NotFound();

            // Get teams for the current season to populate the dropdown
            var currentSeason = _configuration["CurrentSeason"];
            var teamsInLeague = await _teamService.GetTeamsByLeagueIdAsync(leagueId, currentSeason); // This method needs to be season-aware

            ViewBag.Teams = new SelectList(teamsInLeague, "Id", "Name");
            ViewBag.LeagueName = league.Name;

            // Pre-populate the model with the leagueId and currentSeason
            var model = new CreatePointDeductionViewModel
            {
                LeagueId = leagueId,
                Season = currentSeason
            };

            return View(model);
        }
    }
}