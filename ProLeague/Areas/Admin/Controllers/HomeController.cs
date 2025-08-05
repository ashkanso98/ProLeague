// ProLeague/Areas/Admin/Controllers/HomeController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.Admin;

namespace ProLeague.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly IAdminDashboardService _dashboardService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IAdminDashboardService dashboardService, ILogger<HomeController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboardData = await _dashboardService.GetDashboardDataAsync();
                return View(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading the admin dashboard.");
                TempData["ErrorMessage"] = "An error occurred while loading dashboard data.";
                return View(new AdminDashboardViewModel()); // Return an empty model on failure
            }
        }
    }
}