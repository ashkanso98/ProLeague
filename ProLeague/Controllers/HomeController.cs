// FootballNews.Web/Controllers/HomeController.cs
using ProLeague.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProLeague.Domain.Entities;

namespace ProLeague.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get latest 10 news
            var latestNews = await _context.News
                .OrderByDescending(n => n.PublishedDate)
                .Take(10)
                .ToListAsync();

            // Get featured news for the main headline
            var featuredNews = await _context.News
                .Where(n => n.IsFeatured)
                .OrderByDescending(n => n.PublishedDate)
                .ToListAsync();

            // Get leagues to display tables (example: top 3)
            var leagues = await _context.Leagues
                .Include(l => l.Teams)
                .Take(3)
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                LatestNews = latestNews,
                FeaturedNews = featuredNews,
                Leagues = leagues
            };

            return View(viewModel);
        }

        // Other actions like Error, Privacy etc.
    }

    public class HomeViewModel
    {
        public List<News> LatestNews { get; set; } = new List<News>();
        public List<News> FeaturedNews { get; set; } = new List<News>();
        public List<League> Leagues { get; set; } = new List<League>();
    }
}