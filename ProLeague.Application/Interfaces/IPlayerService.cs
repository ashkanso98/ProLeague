// ProLeague.Application/Interfaces/IPlayerService.cs
using ProLeague.Application.ViewModels.Player;
using ProLeague.Domain.Entities;

namespace ProLeague.Application.Interfaces
{
    public interface IPlayerService
    {
        Task<IEnumerable<Player>> GetAllPlayersAsync();
        Task<Player?> GetPlayerByIdAsync(int id);
        Task<EditPlayerViewModel?> GetPlayerForEditAsync(int id);
        Task<Result> CreatePlayerAsync(CreatePlayerViewModel model);
        Task<Result> UpdatePlayerAsync(EditPlayerViewModel model);
        Task<Result> DeletePlayerAsync(int id);
        Task<Player?> GetPlayerWithTeamDetailsAsync(int id);
        Task<IEnumerable<Player>> GetAllPlayersWithTeamAsync();
    }
}