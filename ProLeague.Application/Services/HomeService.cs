using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.Home;
using ProLeague.Domain.Entities; // Required for List<League>

namespace ProLeague.Application.Services
{
    public class HomeService : IHomeService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;

        public HomeService(IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }

        public async Task<HomeViewModel> GetHomeViewModelAsync()
        {
            // Read settings from appsettings.json
            var numberOfLeagues = _configuration.GetValue<int>("HomepageSettings:NumberOfLeaguesToShow");
            var pinnedLeagueId = _configuration.GetValue<int>("HomepageSettings:PinnedLeagueId");

            // Each task runs in its own isolated scope
            var pinnedLeagueTask = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                return await unitOfWork.Leagues.GetLeagueDetailsAsync(pinnedLeagueId);
            });

            var topLeaguesTask = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                return await unitOfWork.Leagues.GetTopLeaguesAsync(numberOfLeagues, pinnedLeagueId);
            });

            var newsTask = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                return await unitOfWork.News.GetRecentNewsAsync(3);
            });

            // Wait for all parallel tasks to complete
            await Task.WhenAll(pinnedLeagueTask, topLeaguesTask, newsTask);

            var news = await newsTask;
            var pinnedLeague = await pinnedLeagueTask;
            var topLeagues = await topLeaguesTask;

            return new HomeViewModel
            {
                FeaturedNews = news.FirstOrDefault(n => n.IsFeatured),
                LatestNews = news.ToList(),
                PinnedLeague = pinnedLeague,
                TopLeagues = topLeagues
            };
        }
    }
}