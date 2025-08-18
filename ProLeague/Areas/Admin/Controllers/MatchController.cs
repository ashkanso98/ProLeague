using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.Match;

namespace ProLeague.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class MatchController : Controller
    {
        private readonly IMatchService _matchService;
        private readonly ILeagueService _leagueService;
        private readonly ITeamService _teamService;

        public MatchController(IMatchService matchService, ILeagueService leagueService, ITeamService teamService)
        {
            _matchService = matchService;
            _leagueService = leagueService;
            _teamService = teamService;
        }

        // GET: Admin/Match
        public async Task<IActionResult> Index()
        {
            var matches = await _matchService.GetAllMatchesWithDetailsAsync(); // متد جدید
            return View(matches);
        }

        // GET: Admin/Match/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Leagues = new SelectList(await _leagueService.GetAllLeaguesAsync(), "Id", "Name");
            return View(new CreateMatchViewModel());
        }

        // POST: Admin/Match/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMatchViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _matchService.CreateMatchAsync(model);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "مسابقه با موفقیت ایجاد شد.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, result.Errors.First());
            }
            ViewBag.Leagues = new SelectList(await _leagueService.GetAllLeaguesAsync(), "Id", "Name", model.LeagueId);
            return View(model);
        }

        // GET: Admin/Match/UpdateResult/5
        public async Task<IActionResult> UpdateResult(int id)
        {
            var match = await _matchService.GetMatchByIdAsync(id);
            if (match == null) return NotFound();

            var model = new UpdateMatchResultViewModel
            {
                MatchId = match.Id,
                HomeTeamGoals = match.HomeTeamGoals ?? 0,
                AwayTeamGoals = match.AwayTeamGoals ?? 0
            };
            ViewBag.Match = match; // ارسال اطلاعات بازی برای نمایش
            return View(model);
        }

        // POST: Admin/Match/UpdateResult/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateResult(UpdateMatchResultViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _matchService.UpdateMatchResultAsync(model);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "نتیجه مسابقه با موفقیت ثبت شد.";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError(string.Empty, result.Errors.First());
            }
            var match = await _matchService.GetMatchByIdAsync(model.MatchId);
            ViewBag.Match = match;
            return View(model);
        }

        // POST: Admin/Match/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            await _matchService.CancelMatchAsync(id);
            TempData["SuccessMessage"] = "وضعیت مسابقه به 'لغو شده' تغییر یافت.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Match/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var match = await _matchService.GetMatchByIdAsync(id);
            if (match == null) return NotFound();
            return View(match);
        }

        // POST: Admin/Match/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _matchService.DeleteMatchAsync(id);
            TempData["SuccessMessage"] = "مسابقه با موفقیت حذف شد.";
            return RedirectToAction(nameof(Index));
        }

        //// API Endpoint for dynamic dropdowns
        //[HttpGet]
        //public async Task<JsonResult> GetTeamsByLeague(int leagueId)
        //{
        //    var teams = (await _teamService.GetAllTeamsAsync()).Where(t => t.LeagueId == leagueId);
        //    return Json(teams.Select(t => new { id = t.Id, name = t.Name }));
        //}
        [HttpGet]
        public async Task<JsonResult> GetTeamsByLeague(int leagueId)
        {
            var teams = await _teamService.GetTeamsByLeagueIdAsync(leagueId);
            return Json(teams.Select(t => new { id = t.Id, name = t.Name }));
        }
    }
}