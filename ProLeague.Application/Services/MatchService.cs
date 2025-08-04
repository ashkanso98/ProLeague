// ProLeague.Application/Services/MatchService.cs
using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.Match;
using ProLeague.Domain.Entities;

namespace ProLeague.Application.Services
{
    public class MatchService : IMatchService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MatchService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Match>> GetMatchesByWeekAsync(int leagueId, int week)
        {
            return await _unitOfWork.Matches.GetMatchesByWeekAsync(leagueId, week);
        }

        public async Task<Match?> GetMatchByIdAsync(int id)
        {
            return await _unitOfWork.Matches.GetByIdAsync(id);
        }

        public async Task<Result> CreateMatchAsync(CreateMatchViewModel model)
        {
            if (model.HomeTeamId == model.AwayTeamId)
            {
                return Result.Failure(new[] { "تیم میزبان و میهمان نمی‌توانند یکسان باشند." });
            }

            var match = new Match
            {
                LeagueId = model.LeagueId,
                HomeTeamId = model.HomeTeamId,
                AwayTeamId = model.AwayTeamId,
                MatchDate = model.MatchDate,
                MatchWeek = model.MatchWeek
            };

            await _unitOfWork.Matches.AddAsync(match);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        public async Task<Result> UpdateMatchResultAsync(UpdateMatchResultViewModel model)
        {
            var match = await _unitOfWork.Matches.GetByIdAsync(model.MatchId);
            if (match == null) return Result.Failure(new[] { "مسابقه یافت نشد." });

            match.HomeTeamGoals = model.HomeTeamGoals;
            match.AwayTeamGoals = model.AwayTeamGoals;

            _unitOfWork.Matches.Update(match);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        public async Task<Result> DeleteMatchAsync(int id)
        {
            var match = await _unitOfWork.Matches.GetByIdAsync(id);
            if (match == null) return Result.Failure(new[] { "مسابقه یافت نشد." });

            _unitOfWork.Matches.Delete(match);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }
    }
}