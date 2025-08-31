using ProLeague.Application.ViewModels.Admin;
using ProLeague.Domain.Entities;

namespace ProLeague.Application.Interfaces
{
    public interface IPointDeductionService
    {
        Task<IEnumerable<PointDeduction>> GetDeductionsByLeagueAsync(int leagueId);
        Task<PointDeduction?> GetDeductionByIdAsync(int id);
        Task<Result> CreateDeductionAsync(CreatePointDeductionViewModel model);
        Task<Result> DeleteDeductionAsync(int id);
    }
}