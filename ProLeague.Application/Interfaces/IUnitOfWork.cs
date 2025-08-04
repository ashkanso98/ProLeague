// ProLeague.Application/Interfaces/IUnitOfWork.cs
using ProLeague.Domain.Entities;

namespace ProLeague.Application.Interfaces
{
    // --- تعریف اینترفیس‌های اختصاصی برای هر ریپازیتوری ---

    //public interface ILeagueRepository : IRepository<League> { }
    public interface ILeagueRepository : IRepository<League>
    {
        Task<IEnumerable<League>> GetLeaguesWithTeamsAsync(int count); 
        Task<League?> GetLeagueDetailsAsync(int id); // **Add this line**
    }
    public interface ITeamRepository : IRepository<Team>
    {
        Task<Team?> GetTeamDetailsAsync(int id);
    }


    public interface IPlayerRepository : IRepository<Player>
    {
        Task<Player?> GetPlayerWithTeamDetailsAsync(int id); 
    }

    public interface INewsRepository : IRepository<News>
    {
        Task<News?> GetNewsDetailsAsync(int id);
        Task<NewsImage?> GetNewsImageByIdAsync(int id);
        void DeleteNewsImage(NewsImage image);
    }

    public interface ICommentRepository : IRepository<NewsComment>
    {
        Task<IEnumerable<NewsComment>> GetCommentsByStatusAsync(CommentStatus status);
    }

    public interface IMatchRepository : IRepository<Match>
    {
        Task<IEnumerable<Match>> GetMatchesByWeekAsync(int leagueId, int week);
    }
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        // این متد برای Include کردن تیم محبوب لازم است
        Task<ApplicationUser?> GetUserWithFavoriteTeamAsync(string id);
    }

    // --- اینترفیس اصلی Unit of Work ---

    public interface IUnitOfWork : IDisposable
    {
        ILeagueRepository Leagues { get; }
        ITeamRepository Teams { get; }
        IPlayerRepository Players { get; }
        INewsRepository News { get; }
        ICommentRepository Comments { get; }
        IMatchRepository Matches { get; }
        IUserRepository Users { get; }

        Task<int> CompleteAsync();
    }
}