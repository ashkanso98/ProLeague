using ProLeague.Domain.Entities;

namespace ProLeague.Application.Interfaces
{
    // --- Specific repository interfaces ---

    public interface ILeagueRepository : IRepository<League>
    {
        Task<IEnumerable<League>> GetLeaguesWithTeamsAsync(int count);
        Task<League?> GetLeagueDetailsAsync(int id);
    }

    public interface ITeamRepository : IRepository<Team>
    {
        Task<Team?> GetTeamDetailsAsync(int id);
    }

    public interface IPlayerRepository : IRepository<Player>
    {
        Task<Player?> GetPlayerWithTeamDetailsAsync(int id);
        Task<IEnumerable<Player>> GetAllPlayersWithTeamAsync();
    }

    public interface INewsRepository : IRepository<News>
    {
        Task<News?> GetNewsDetailsAsync(int id);
        Task<NewsImage?> GetNewsImageByIdAsync(int id);
        void DeleteNewsImage(NewsImage image);
        Task<IEnumerable<News>> GetRecentNewsAsync(int count);
    }

    public interface ICommentRepository : IRepository<NewsComment>
    {
        Task<IEnumerable<NewsComment>> GetCommentsByStatusAsync(CommentStatus status);
        Task<IEnumerable<NewsComment>> GetRecentCommentsAsync(int count);
        Task<NewsComment?> GetCommentDetailsAsync(int id);
    }

    //public interface IMatchRepository : IRepository<Match>
    //{
    //    Task<IEnumerable<Match>> GetMatchesByWeekAsync(int leagueId, int week);
    //}

    public interface IUserRepository : IRepository<ApplicationUser>
    {
        Task<ApplicationUser?> GetUserWithFavoriteTeamAsync(string id);
    }


    public interface IMatchRepository : IRepository<Match>
    {
        Task<IEnumerable<Match>> GetMatchesByWeekAsync(int leagueId, int week);
        Task<IEnumerable<Match>> GetMatchesForTeamAsync(int teamId);
        Task<IEnumerable<Match>> GetAllMatchesWithDetailsAsync();
        Task<Match?> GetMatchWithTeamsByIdAsync(int id);
    }
    // --- Main Unit of Work interface ---

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