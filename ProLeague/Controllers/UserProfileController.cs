// ProLeague/Controllers/UserProfileController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProLeague.Application.Interfaces;
using ProLeague.Domain.Entities;

[Authorize]
public class UserProfileController : Controller
{
    private readonly IUserService _userService;
    private readonly ITeamService _teamService; // برای گرفتن لیست تیم‌ها
    private readonly UserManager<ApplicationUser> _userManager;

    public UserProfileController(IUserService userService, ITeamService teamService, UserManager<ApplicationUser> userManager)
    {
        _userService = userService;
        _teamService = teamService;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var user = await _userService.GetUserWithFavoriteTeamAsync(userId);
        if (user == null) return NotFound();

        ViewBag.Teams = await _teamService.GetAllTeamsAsync();
        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetFavoriteTeam(int? teamId)
    {
        var userId = _userManager.GetUserId(User);
        var result = await _userService.SetFavoriteTeamAsync(userId, teamId);

        if (!result.Succeeded)
        {
            // Handle error
        }
        return RedirectToAction(nameof(Index));
    }
}