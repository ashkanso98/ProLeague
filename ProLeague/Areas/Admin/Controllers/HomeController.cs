using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProLeague.Infrastructure.Data;
using ProLeague.Domain.Entities;
using Microsoft.Extensions.Logging; // اضافه کردن برای لاگ کردن
using ProLeague.Areas.Admin.Models; // اضافه کردن برای ViewModel
using Microsoft.Extensions.DependencyInjection; // این using را اضافه کنید


namespace ProLeague.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger; // برای لاگ کردن خطاها
        private readonly IServiceScopeFactory _scopeFactory;
       

        public HomeController(IServiceScopeFactory scopeFactory, ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // هر وظیفه در یک scope جداگانه اجرا می‌شود تا DbContext مجزا داشته باشد
                var leaguesCountTask = Task.Run(async () => {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    return await context.Leagues.CountAsync();
                });

                var teamsCountTask = Task.Run(async () => {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    return await context.Teams.CountAsync();
                });

                var playersCountTask = Task.Run(async () => {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    return await context.Players.CountAsync();
                });

                var newsCountTask = Task.Run(async () => {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    return await context.News.CountAsync();
                });

                var commentsCountTask = Task.Run(async () => {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    return await context.NewsComments.CountAsync();
                });

                var recentNewsTask = Task.Run(async () => {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    return await context.News
                        .OrderByDescending(n => n.PublishedDate)
                        .Take(5)
                        .ToListAsync();
                });

                var recentCommentsTask = Task.Run(async () => {
                    using var scope = _scopeFactory.CreateScope();
                    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    return await context.NewsComments
                        .Include(c => c.User)
                        .Include(c => c.News)
                        .OrderByDescending(c => c.CreatedDate)
                        .Take(5)
                        .ToListAsync();
                });

                // منتظر تکمیل همه وظایف بمانید
                await Task.WhenAll(
                    leaguesCountTask, teamsCountTask, playersCountTask, newsCountTask,
                    commentsCountTask, recentNewsTask, recentCommentsTask
                );

                var dashboardData = new AdminDashboardViewModel
                {
                    TotalLeagues = await leaguesCountTask,
                    TotalTeams = await teamsCountTask,
                    TotalPlayers = await playersCountTask,
                    TotalNews = await newsCountTask,
                    TotalComments = await commentsCountTask,
                    RecentNews = await recentNewsTask,
                    RecentComments = await recentCommentsTask
                };

                return View(dashboardData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطایی در بارگذاری داشبورد ادمین رخ داد.");
                TempData["ErrorMessage"] = "خطایی در بارگذاری اطلاعات داشبورد رخ داده است. لطفاً بعداً تلاش کنید.";
                return View(new AdminDashboardViewModel());
            }
        }
    }
}