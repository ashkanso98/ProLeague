// ProLeague.Application/Interfaces/ITeamService.cs
using ProLeague.Application.ViewModels.Team;
using ProLeague.Domain.Entities;

namespace ProLeague.Application.Interfaces
{
    public interface ITeamService
    {
        Task<IEnumerable<Team>> GetAllTeamsAsync();
        Task<Team?> GetTeamDetailsAsync(int id);
        Task<EditTeamViewModel?> GetTeamForEditAsync(int id);
        Task<Result> CreateTeamAsync(CreateTeamViewModel model);
        Task<Result> UpdateTeamAsync(EditTeamViewModel model);
        Task<Result> DeleteTeamAsync(int id);
        Task<IEnumerable<Team>> GetTeamsByLeagueIdAsync(int leagueId);
    }
}