using ProLeague.Application.ViewModels.Admin;

namespace ProLeague.Application.Interfaces
{
    public interface IAdminDashboardService
    {
        Task<AdminDashboardViewModel> GetDashboardDataAsync();
    }
}