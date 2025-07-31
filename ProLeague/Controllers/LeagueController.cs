// FootballNews.Web/Controllers/LeagueController.cs
using ProLeague.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProLeague.Controllers
{
    public class LeagueController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeagueController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: League/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var league = await _context.Leagues
                .Include(l => l.Teams)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (league == null)
            {
                return NotFound();
            }

            // Sort teams based on points, goal difference, goals for
            var sortedTeams = league.Teams
                .OrderByDescending(t => t.Points)
                .ThenByDescending(t => t.GoalDifference)
                .ThenByDescending(t => t.GoalsFor)
                .ToList();

            ViewBag.SortedTeams = sortedTeams; // Pass sorted list to view

            return View(league);
        }
    }
}