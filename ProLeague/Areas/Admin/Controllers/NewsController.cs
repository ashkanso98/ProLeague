using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.News;

namespace ProLeague.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class NewsController : Controller
    {
        private readonly INewsService _newsService;
        private readonly ILeagueService _leagueService;
        private readonly ITeamService _teamService;
        private readonly IPlayerService _playerService;

        public NewsController(INewsService newsService, ILeagueService leagueService, ITeamService teamService, IPlayerService playerService)
        {
            _newsService = newsService;
            _leagueService = leagueService;
            _teamService = teamService;
            _playerService = playerService;
        }

        // GET: Admin/News
        public async Task<IActionResult> Index()
        {
            var news = await _newsService.GetAllNewsAsync();
            return View(news);
        }

        // GET: Admin/News/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var news = await _newsService.GetNewsDetailsAsync(id);
            if (news == null) return NotFound();
            return View(news);
        }

        // GET: Admin/News/Create
        public async Task<IActionResult> Create()
        {
            await PopulateRelatedDataForView();
            return View(new CreateNewsViewModel());
        }

        // POST: Admin/News/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateNewsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateRelatedDataForView(model);
                return View(model);
            }

            var result = await _newsService.CreateNewsAsync(model);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "خبر با موفقیت ایجاد شد.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Errors?.FirstOrDefault() ?? "An unknown error occurred.");
            await PopulateRelatedDataForView(model);
            return View(model);
        }

        // GET: Admin/News/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _newsService.GetNewsForEditAsync(id);
            if (model == null) return NotFound();

            await PopulateRelatedDataForView(model);
            return View(model);
        }

        // POST: Admin/News/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditNewsViewModel model)
        {
            if (id != model.Id) return BadRequest();

            if (!ModelState.IsValid)
            {
                await PopulateRelatedDataForView(model);
                return View(model);
            }

            var result = await _newsService.UpdateNewsAsync(model);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "خبر با موفقیت به‌روزرسانی شد.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, result.Errors?.FirstOrDefault() ?? "An unknown error occurred.");
            await PopulateRelatedDataForView(model);
            return View(model);
        }



        // ProLeague/Areas/Admin/Controllers/NewsController.cs

        // ...
        [HttpPost]
        [ValidateAntiForgeryToken] // این Attribute را برای امنیت اضافه کنید
        public async Task<IActionResult> DeleteGalleryImage([FromQuery] int imageId) // [FromQuery] را اضافه کنید
        {
            var result = await _newsService.DeleteGalleryImageAsync(imageId);
            return Json(new { success = result.Succeeded });
        }
        //// ...
        //// POST: Admin/News/DeleteGalleryImage
        //[HttpPost]
        //public async Task<IActionResult> DeleteGalleryImage(int imageId)
        //{
        //    var result = await _newsService.DeleteGalleryImageAsync(imageId);
        //    return Json(new { success = result.Succeeded });
        //}

        // GET: Admin/News/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var news = await _newsService.GetNewsDetailsAsync(id);
            if (news == null) return NotFound();
            return View(news);
        }

        // POST: Admin/News/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _newsService.DeleteNewsAsync(id);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "خبر با موفقیت حذف شد.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Errors?.FirstOrDefault() ?? "خطا در حذف خبر.";
            }
            return RedirectToAction(nameof(Index));
        }

        // Helper method to populate dropdowns for Create/Edit views
        private async Task PopulateRelatedDataForView(dynamic? model = null)
        {
            var leagues = await _leagueService.GetAllLeaguesAsync();
            var teams = await _teamService.GetAllTeamsAsync();
            var players = await _playerService.GetAllPlayersAsync();

            ViewData["Leagues"] = new MultiSelectList(leagues.OrderBy(x => x.Name), "Id", "Name", model?.RelatedLeagueIds);
            ViewData["Teams"] = new MultiSelectList(teams.OrderBy(x => x.Name), "Id", "Name", model?.RelatedTeamIds);
            ViewData["Players"] = new MultiSelectList(players.OrderBy(x => x.Name), "Id", "Name", model?.RelatedPlayerIds);
        }
    }
}