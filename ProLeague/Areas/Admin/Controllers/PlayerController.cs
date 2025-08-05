using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.Player;

namespace ProLeague.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class PlayerController : Controller
    {
        private readonly IPlayerService _playerService;
        private readonly ITeamService _teamService; // Needed for the dropdown list

        public PlayerController(IPlayerService playerService, ITeamService teamService)
        {
            _playerService = playerService;
            _teamService = teamService;
        }

        // GET: Admin/Player
        public async Task<IActionResult> Index()
        {
            var players = await _playerService.GetAllPlayersWithTeamAsync();
            return View(players);
        }

        // GET: Admin/Player/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var player = await _playerService.GetPlayerWithTeamDetailsAsync(id);
            if (player == null) return NotFound();
            return View(player);
        }

        // GET: Admin/Player/Create
        public async Task<IActionResult> Create()
        {
            await PopulateTeamsDropDownList();
            return View(new CreatePlayerViewModel());
        }

        // POST: Admin/Player/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePlayerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateTeamsDropDownList(model.TeamId);
                return View(model);
            }

            var result = await _playerService.CreatePlayerAsync(model);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "بازیکن با موفقیت ایجاد شد.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Errors?.FirstOrDefault() ?? "An unknown error occurred.");
            await PopulateTeamsDropDownList(model.TeamId);
            return View(model);
        }

        // GET: Admin/Player/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _playerService.GetPlayerForEditAsync(id);
            if (model == null) return NotFound();

            await PopulateTeamsDropDownList(model.TeamId);
            return View(model);
        }

        // POST: Admin/Player/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditPlayerViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateTeamsDropDownList(model.TeamId);
                return View(model);
            }

            var result = await _playerService.UpdatePlayerAsync(model);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "اطلاعات بازیکن با موفقیت به‌روزرسانی شد.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Errors?.FirstOrDefault() ?? "An unknown error occurred.");
            await PopulateTeamsDropDownList(model.TeamId);
            return View(model);
        }

        // GET: Admin/Player/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var player = await _playerService.GetPlayerWithTeamDetailsAsync(id);
            if (player == null) return NotFound();
            return View(player);
        }

        // POST: Admin/Player/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _playerService.DeletePlayerAsync(id);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "بازیکن با موفقیت حذف شد.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "خطا در حذف بازیکن.";
            }
            return RedirectToAction(nameof(Index));
        }

        // Helper method to populate the teams dropdown
        private async Task PopulateTeamsDropDownList(object? selectedTeam = null)
        {
            var teams = await _teamService.GetAllTeamsAsync();
            ViewBag.TeamId = new SelectList(teams.OrderBy(t => t.Name), "Id", "Name", selectedTeam);
        }
    }
}