// FootballNews.Web/Controllers/UserProfileController.cs
using ProLeague.Domain.Entities;
using ProLeague.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ProLeague.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfileController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var teams = await _context.Teams.ToListAsync();
            ViewBag.Teams = teams;

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetFavoriteTeam(int? teamId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Validate teamId if provided
            if (teamId.HasValue)
            {
                var teamExists = await _context.Teams.AnyAsync(t => t.Id == teamId.Value);
                if (!teamExists)
                {
                    ModelState.AddModelError("", "Selected team does not exist.");
                    var teams = await _context.Teams.ToListAsync();
                    ViewBag.Teams = teams;
                    return View(nameof(Index), user); // Return to profile view with error
                }
            }

            user.FavoriteTeamId = teamId; // Can be null to unset

            // Update fan count logic
            // Decrement old team's fan count if it existed
            if (user.FavoriteTeamId.HasValue && user.FavoriteTeamId != teamId)
            {
                var oldTeam = await _context.Teams.FindAsync(user.FavoriteTeamId);
                if (oldTeam != null)
                {
                    oldTeam.FanCount = Math.Max(0, oldTeam.FanCount - 1);
                }
            }
            // Increment new team's fan count if selected
            if (teamId.HasValue)
            {
                var newTeam = await _context.Teams.FindAsync(teamId.Value);
                if (newTeam != null)
                {
                    newTeam.FanCount++;
                }
            }


            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync(); // Save changes to team fan counts

            return RedirectToAction(nameof(Index));
        }
    }
}