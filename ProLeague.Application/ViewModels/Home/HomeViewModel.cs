// ProLeague.Application/ViewModels/Home/HomeViewModel.cs
using ProLeague.Domain.Entities;

namespace ProLeague.Application.ViewModels.Home
{
    public class HomeViewModel
    {
        public List<Domain.Entities.News> LatestNews { get; set; } = new();
        public Domain.Entities.News? FeaturedNews { get; set; } // فقط یک خبر ویژه اصلی
        public List<Domain.Entities.League> Leagues { get; set; } = new();
        // پراپرتی جدید برای لیگ پین‌شده
        public Domain.Entities.League? PinnedLeague { get; set; }

        // تغییر نام این پراپرتی برای وضوح بیشتر
        public List<Domain.Entities.League> TopLeagues { get; set; } = new();
        public IEnumerable<Domain.Entities.Match> UpcomingMatches { get; set; } = new List<Domain.Entities.Match>();
    }
}