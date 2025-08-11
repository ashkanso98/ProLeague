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

        public async Task<IEnumerable<Match>> GetAllMatchesWithDetailsAsync()
        {
            return await _unitOfWork.Matches.GetAllMatchesWithDetailsAsync();
        }
        public async Task<Result> CreateMatchAsync(CreateMatchViewModel model)
        {
            if (model.HomeTeamId == model.AwayTeamId)
            {
                return Result.Failure(new[] { "Home and away teams cannot be the same." });
            }

            var match = new Match
            {
                LeagueId = model.LeagueId,
                HomeTeamId = model.HomeTeamId,
                AwayTeamId = model.AwayTeamId,
                MatchDate = model.MatchDate,
                MatchWeek = model.MatchWeek
            };

            // Check if scores were entered
            if (model.HomeTeamGoals.HasValue && model.AwayTeamGoals.HasValue)
            {
                match.HomeTeamGoals = model.HomeTeamGoals.Value;
                match.AwayTeamGoals = model.AwayTeamGoals.Value;
                match.Status = MatchStatus.Finished; // Set status to Finished if scores are provided
            }
            else
            {
                match.Status = MatchStatus.Scheduled; // Otherwise, it's just scheduled
            }

            await _unitOfWork.Matches.AddAsync(match);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        public async Task<Result> UpdateMatchResultAsync(UpdateMatchResultViewModel model)
        {
            var match = await _unitOfWork.Matches.GetByIdAsync(model.MatchId);
            if (match == null) return Result.Failure(new[] { "Match not found." });

            match.HomeTeamGoals = model.HomeTeamGoals;
            match.AwayTeamGoals = model.AwayTeamGoals;
            match.Status = MatchStatus.Finished;

            _unitOfWork.Matches.Update(match);
            await _unitOfWork.CompleteAsync();

            // Team stats are calculated properties, so no need to update them here!

            return Result.Success();
        }

        public async Task<Result> CancelMatchAsync(int id)
        {
            var match = await _unitOfWork.Matches.GetByIdAsync(id);
            if (match == null) return Result.Failure(new[] { "Match not found." });

            match.Status = MatchStatus.Canceled;
            _unitOfWork.Matches.Update(match);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        public async Task<Result> DeleteMatchAsync(int id)
        {
            var match = await _unitOfWork.Matches.GetByIdAsync(id);
            if (match == null) return Result.Failure(new[] { "Match not found." });

            _unitOfWork.Matches.Delete(match);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        public async Task<IEnumerable<Match>> GetMatchesByWeekAsync(int leagueId, int week)
        {
            return await _unitOfWork.Matches.GetMatchesByWeekAsync(leagueId, week);
        }

        public async Task<IEnumerable<Match>> GetMatchesForTeamAsync(int teamId)
        {
            return await _unitOfWork.Matches.GetMatchesForTeamAsync(teamId);
        }

        public async Task<Match?> GetMatchByIdAsync(int id)
        {
            //return await _unitOfWork.Matches.GetByIdAsync(id);
            return await _unitOfWork.Matches.GetMatchWithTeamsByIdAsync(id);
        }
    }
}