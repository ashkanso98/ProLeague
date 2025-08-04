// ProLeague.Application/Interfaces/IHomeService.cs
using ProLeague.Application.ViewModels.Home;

namespace ProLeague.Application.Interfaces
{
    public interface IHomeService
    {
        Task<HomeViewModel> GetHomeViewModelAsync();
    }
}