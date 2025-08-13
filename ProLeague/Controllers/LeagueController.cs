using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> Details(int id)
    {
        // از متد جدید استفاده می‌کنیم
        var league = await _leagueService.GetLeagueDetailsAsync(id);
        if (league == null)
        {
            return NotFound();
        }
        return View(league);
    }
}