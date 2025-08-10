using ProLeague.Application.ViewModels.Match;
using ProLeague.Domain.Entities;

namespace ProLeague.Application.Interfaces
{
    public interface IMatchService
    {
        Task<IEnumerable<Match>> GetMatchesByWeekAsync(int leagueId, int week);
        Task<IEnumerable<Match>> GetMatchesForTeamAsync(int teamId);
        Task<Match?> GetMatchByIdAsync(int id);
        Task<Result> CreateMatchAsync(CreateMatchViewModel model);
        Task<Result> UpdateMatchResultAsync(UpdateMatchResultViewModel model);
        Task<Result> CancelMatchAsync(int id);
        Task<IEnumerable<Match>> GetAllMatchesWithDetailsAsync();
        Task<Result> DeleteMatchAsync(int id);
    }
}