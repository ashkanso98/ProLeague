using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using ProLeague.Application.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace ProLeague.Controllers
{
    public class LeagueController : Controller
    {
        private readonly ILeagueService _leagueService;
        private readonly IConfiguration _configuration;

        public LeagueController(ILeagueService leagueService, IConfiguration configuration)
        {
            _leagueService = leagueService;
            _configuration = configuration;
        }

        // GET: /League
        public async Task<IActionResult> Index()
        {
            // Read the current season from configuration
            var currentSeason = _configuration["CurrentSeason"];
            // Pass the season to the service to get the correct list of leagues
            var leagues = await _leagueService.GetAllLeaguesWithTeamsAsync(currentSeason);
            return View(leagues);
        }

        // GET: /League/Details/5
        public async Task<IActionResult> Details(int id, int? week)
        {
            // Read the current season from configuration
            var currentSeason = _configuration["CurrentSeason"];
            // Pass both the id and the season to the service
            var league = await _leagueService.GetLeagueDetailsAsync(id, currentSeason);

            if (league == null)
            {
                return NotFound();
            }

            var availableWeeks = league.Matches
                .Select(m => m.MatchWeek)
                .Distinct()
                .OrderBy(w => w);

            ViewData["AvailableWeeks"] = new SelectList(availableWeeks);
            ViewData["SelectedWeek"] = week ?? availableWeeks.LastOrDefault();

            return View(league);
        }
    }
}