//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using ProLeague.Application.Interfaces;
//using ProLeague.Application.ViewModels.Home;
//using ProLeague.Domain.Entities; // Required for List<League>

//namespace ProLeague.Application.Services
//{
//    public class HomeService : IHomeService
//    {
//        private readonly IServiceScopeFactory _scopeFactory;
//        private readonly IConfiguration _configuration;

//        public HomeService(IServiceScopeFactory scopeFactory, IConfiguration configuration)
//        {
//            _scopeFactory = scopeFactory;
//            _configuration = configuration;
//        }

//        public async Task<HomeViewModel> GetHomeViewModelAsync()
//        {
//            // Read settings from appsettings.json
//            var numberOfLeagues = _configuration.GetValue<int>("HomepageSettings:NumberOfLeaguesToShow");
//            var pinnedLeagueId = _configuration.GetValue<int>("HomepageSettings:PinnedLeagueId");

//            // Each task runs in its own isolated scope
//            var pinnedLeagueTask = Task.Run(async () =>
//            {
//                using var scope = _scopeFactory.CreateScope();
//                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
//                return await unitOfWork.Leagues.GetLeagueDetailsAsync(pinnedLeagueId);
//            });
//            var upcomingMatchesTask = Task.Run(async () =>
//            {
//                using var scope = _scopeFactory.CreateScope();
//                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
//                return await unitOfWork.Matches.GetUpcomingMatchesAsync(DateTime.UtcNow.AddDays(7));
//            });
//            var topLeaguesTask = Task.Run(async () =>
//            {
//                using var scope = _scopeFactory.CreateScope();
//                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
//                return await unitOfWork.Leagues.GetTopLeaguesAsync(numberOfLeagues, pinnedLeagueId);
//            });

//            var newsTask = Task.Run(async () =>
//            {
//                using var scope = _scopeFactory.CreateScope();
//                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
//                return await unitOfWork.News.GetRecentNewsAsync(3);
//            });

//            // Wait for all parallel tasks to complete
//            await Task.WhenAll(pinnedLeagueTask, topLeaguesTask, newsTask);

//            var news = await newsTask;
//            var pinnedLeague = await pinnedLeagueTask;
//            var topLeagues = await topLeaguesTask;

//            return new HomeViewModel
//            {
//                FeaturedNews = news.FirstOrDefault(n => n.IsFeatured),
//                LatestNews = news.ToList(),
//                PinnedLeague = pinnedLeague,
//                TopLeagues = topLeagues
//            };
//        }
//    }
//}
using Microsoft.Extensions.Configuration;
using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.Home;
using ProLeague.Domain.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace ProLeague.Application.Services
{
    public class HomeService : IHomeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        // We no longer need IServiceScopeFactory here, just IUnitOfWork
        public HomeService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<HomeViewModel> GetHomeViewModelAsync()
        {
            // Read settings from appsettings.json
            var numberOfLeagues = _configuration.GetValue<int>("HomepageSettings:NumberOfLeaguesToShow");
            var pinnedLeagueId = _configuration.GetValue<int>("HomepageSettings:PinnedLeagueId");

            // --- Execute queries sequentially ---
            var news = await _unitOfWork.News.GetRecentNewsAsync(3);
            var pinnedLeague = await _unitOfWork.Leagues.GetLeagueDetailsAsync(pinnedLeagueId);
            var topLeagues = await _unitOfWork.Leagues.GetTopLeaguesAsync(numberOfLeagues, pinnedLeagueId);
            var upcomingMatches = await _unitOfWork.Matches.GetUpcomingMatchesAsync(DateTime.UtcNow.AddDays(7));

            return new HomeViewModel
            {
                FeaturedNews = news.FirstOrDefault(n => n.IsFeatured),
                LatestNews = news.ToList(),
                PinnedLeague = pinnedLeague,
                TopLeagues = topLeagues,
                UpcomingMatches = upcomingMatches
            };
        }
    }
}