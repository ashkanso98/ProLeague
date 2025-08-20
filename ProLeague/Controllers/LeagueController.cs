using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProLeague.Application.Interfaces;

public class LeagueController : Controller
{
    private readonly ILeagueService _leagueService;

    public LeagueController(ILeagueService leagueService)
    {
        _leagueService = leagueService;
    }
    // GET: /League
    public async Task<IActionResult> Index()
    {
        var leagues = await _leagueService.GetAllLeaguesWithTeamsAsync();
        return View(leagues);
    }
    public async Task<IActionResult> Details(int id, int? week)
    {
        var league = await _leagueService.GetLeagueDetailsAsync(id);
        if (league == null)
        {
            return NotFound();
        }

        // Get all unique match weeks to populate the dropdown
        var availableWeeks = league.Matches
            .Select(m => m.MatchWeek)
            .Distinct()
            .OrderBy(w => w);

        // Pass the list of weeks to the view
        ViewData["AvailableWeeks"] = new SelectList(availableWeeks);

        // Determine which week to display. If no week is selected in the URL,
        // default to the most recently played/scheduled week.
        ViewData["SelectedWeek"] = week ?? availableWeeks.LastOrDefault();

        return View(league);
    }
}