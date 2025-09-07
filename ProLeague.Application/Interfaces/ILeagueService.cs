// ProLeague.Application/Interfaces/ILeagueService.cs
using ProLeague.Application.ViewModels.League;
using ProLeague.Domain.Entities;

namespace ProLeague.Application.Interfaces
{
    public interface ILeagueService
    {
        Task<IEnumerable<League>> GetAllLeaguesAsync();
        Task<League?> GetLeagueByIdAsync(int id);
        Task<EditLeagueViewModel?> GetLeagueForEditAsync(int id);
        Task<Result> CreateLeagueAsync(CreateLeagueViewModel model);
        Task<Result> UpdateLeagueAsync(EditLeagueViewModel model);
        Task<Result> DeleteLeagueAsync(int id);
        Task<League?> GetLeagueDetailsAsync(int id); 
        Task<IEnumerable<League>> GetAllLeaguesWithTeamsAsync();

        Task<IEnumerable<League>> GetAllLeaguesWithTeamsAsync(string season);
        Task<League?> GetLeagueDetailsAsync(int id, string season);

    }
}