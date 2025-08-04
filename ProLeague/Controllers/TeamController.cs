// ProLeague/Controllers/TeamController.cs
using Microsoft.AspNetCore.Mvc;
using ProLeague.Application.Interfaces;

namespace ProLeague.Controllers
{
    public class TeamController : Controller
    {
        private readonly ITeamService _teamService;

        public TeamController(ITeamService teamService)
        {
            _teamService = teamService;
        }

        // GET: Team/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var team = await _teamService.GetTeamDetailsAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }
    }
}