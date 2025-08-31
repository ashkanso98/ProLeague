using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.Admin;
using ProLeague.Domain.Entities;

namespace ProLeague.Application.Services
{
    public class PointDeductionService : IPointDeductionService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PointDeductionService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PointDeduction>> GetDeductionsByLeagueAsync(int leagueId)
        {
            return await _unitOfWork.PointDeductions.GetDeductionsByLeagueAsync(leagueId);
        }

        public async Task<PointDeduction?> GetDeductionByIdAsync(int id)
        {
            return await _unitOfWork.PointDeductions.GetDeductionDetailsAsync(id);
        }

        public async Task<Result> CreateDeductionAsync(CreatePointDeductionViewModel model)
        {
            // Check if the team is actually in the league
            var leagueEntry = await _unitOfWork.LeagueEntries.FindAsync(model.TeamId, model.LeagueId);
            if (leagueEntry == null)
            {
                return Result.Failure(new[] { "The selected team is not part of the selected league." });
            }

            var deduction = new PointDeduction
            {
                TeamId = model.TeamId,
                LeagueId = model.LeagueId,
                Points = model.Points,
                Reason = model.Reason,
                DateApplied = DateTime.UtcNow
            };

            await _unitOfWork.PointDeductions.AddAsync(deduction);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        public async Task<Result> DeleteDeductionAsync(int id)
        {
            var deduction = await _unitOfWork.PointDeductions.GetByIdAsync(id);
            if (deduction == null)
            {
                return Result.Failure(new[] { "Deduction not found." });
            }

            _unitOfWork.PointDeductions.Delete(deduction);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }
    }
}