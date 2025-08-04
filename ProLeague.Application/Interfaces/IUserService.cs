// ProLeague.Application/Interfaces/IUserService.cs
using ProLeague.Application.ViewModels.User;
using ProLeague.Domain.Entities;

namespace ProLeague.Application.Interfaces
{
    public interface IUserService
    {
        Task<List<UserRolesViewModel>> GetAllUsersWithRolesAsync();
        Task<ManageUserRolesViewModel?> GetUserForManagingRolesAsync(string userId);
        Task<Result> UpdateUserRolesAsync(string userId, List<string> selectedRoles);
        Task<Result> SetFavoriteTeamAsync(string userId, int? teamId);
        Task<ApplicationUser?> GetUserWithFavoriteTeamAsync(string userId);
    }
}