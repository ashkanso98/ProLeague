using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.Home;

public class HomeService : IHomeService
{
    private readonly IUnitOfWork _unitOfWork;
    public HomeService(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<HomeViewModel> GetHomeViewModelAsync()
    {
        // ... (منطق دریافت اخبار)
        var allNews = await _unitOfWork.News.GetAllAsync();

        // --- این بخش را اصلاح کنید ---
        var leaguesWithTeams = await _unitOfWork.Leagues.GetLeaguesWithTeamsAsync(3);

        return new HomeViewModel
        {
            LatestNews = allNews.OrderByDescending(n => n.PublishedDate).Take(10).ToList(),
            FeaturedNews = allNews.FirstOrDefault(n => n.IsFeatured),
            Leagues = leaguesWithTeams.ToList() // استفاده از لیست جدید
        };
    }
}