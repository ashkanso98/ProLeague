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

        public HomeService(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<HomeViewModel> GetHomeViewModelAsync()
        {
            // 1. Read settings from appsettings.json
            var currentSeason = _configuration["CurrentSeason"];
            var numberOfLeagues = _configuration.GetValue<int>("HomepageSettings:NumberOfLeaguesToShow");
            var pinnedLeagueId = _configuration.GetValue<int>("HomepageSettings:PinnedLeagueId");

            // 2. Fetch all necessary data sequentially
            var news = await _unitOfWork.News.GetRecentNewsAsync(3); // Fetch last 3 news articles

            var upcomingMatches = await _unitOfWork.Matches.GetUpcomingMatchesAsync(DateTime.UtcNow.AddDays(7));

            // Fetch leagues for the homepage, filtered by the current season
            var homepageLeagues = await _unitOfWork.Leagues.GetHomepageLeaguesAsync(numberOfLeagues, pinnedLeagueId, currentSeason);

            // 3. Build the ViewModel
            return new HomeViewModel
            {
                FeaturedNews = news.FirstOrDefault(n => n.IsFeatured),
                LatestNews = news.ToList(),
                PinnedLeague = homepageLeagues.FirstOrDefault(l => l.Id == pinnedLeagueId),
                TopLeagues = homepageLeagues.Where(l => l.Id != pinnedLeagueId).ToList(),
                UpcomingMatches = upcomingMatches
            };
        }
    }
}