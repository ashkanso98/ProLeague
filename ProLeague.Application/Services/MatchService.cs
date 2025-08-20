using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.Match;
using ProLeague.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProLeague.Application.Services
{
    public class MatchService : IMatchService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MatchService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> UpdateMatchResultAsync(UpdateMatchResultViewModel model)
        {
            var match = await _unitOfWork.Matches.GetByIdAsync(model.MatchId);
            if (match == null) return Result.Failure(new[] { "Match not found." });

            var homeTeamEntry = await _unitOfWork.LeagueEntries.FindAsync(match.HomeTeamId, match.LeagueId);
            var awayTeamEntry = await _unitOfWork.LeagueEntries.FindAsync(match.AwayTeamId, match.LeagueId);

            if (homeTeamEntry == null || awayTeamEntry == null)
            {
                return Result.Failure(new[] { "Team entries in the league table could not be found." });
            }

            if (match.IsFinished)
            {
                int oldHomeGoals = match.HomeTeamGoals.Value;
                int oldAwayGoals = match.AwayTeamGoals.Value;

                RevertStats(homeTeamEntry, oldHomeGoals, oldAwayGoals);
                RevertStats(awayTeamEntry, oldAwayGoals, oldHomeGoals);
            }

            match.HomeTeamGoals = model.HomeTeamGoals;
            match.AwayTeamGoals = model.AwayTeamGoals;
            match.Status = MatchStatus.Finished;

            // CORRECTED LINES BELOW (removed .Value)
            ApplyStats(homeTeamEntry, model.HomeTeamGoals.Value, model.AwayTeamGoals.Value);
            ApplyStats(awayTeamEntry, model.AwayTeamGoals.Value, model.HomeTeamGoals.Value);

            await _unitOfWork.CompleteAsync();
            return Result.Success();
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

            await _unitOfWork.Matches.AddAsync(match);

            if (model.HomeTeamGoals.HasValue && model.AwayTeamGoals.HasValue)
            {
                match.HomeTeamGoals = model.HomeTeamGoals.Value;
                match.AwayTeamGoals = model.AwayTeamGoals.Value;
                match.Status = MatchStatus.Finished;

                var homeEntry = await _unitOfWork.LeagueEntries.FindAsync(match.HomeTeamId, match.LeagueId);
                var awayEntry = await _unitOfWork.LeagueEntries.FindAsync(match.AwayTeamId, match.LeagueId);

                if (homeEntry != null && awayEntry != null)
                {
                    ApplyStats(homeEntry, model.HomeTeamGoals.Value, model.AwayTeamGoals.Value);
                    ApplyStats(awayEntry, model.AwayTeamGoals.Value, model.HomeTeamGoals.Value);
                }
            }

            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        public async Task<Result> CancelMatchAsync(int id)
        {
            var match = await _unitOfWork.Matches.GetByIdAsync(id);
            if (match == null) return Result.Failure(new[] { "Match not found." });

            if (match.IsFinished)
            {
                var homeTeamEntry = await _unitOfWork.LeagueEntries.FindAsync(match.HomeTeamId, match.LeagueId);
                var awayTeamEntry = await _unitOfWork.LeagueEntries.FindAsync(match.AwayTeamId, match.LeagueId);
                if (homeTeamEntry != null && awayTeamEntry != null)
                {
                    RevertStats(homeTeamEntry, match.HomeTeamGoals.Value, match.AwayTeamGoals.Value);
                    RevertStats(awayTeamEntry, match.AwayTeamGoals.Value, match.HomeTeamGoals.Value);
                }
            }

            match.Status = MatchStatus.Canceled;
            match.HomeTeamGoals = null;
            match.AwayTeamGoals = null;

            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        public async Task<Result> DeleteMatchAsync(int id)
        {
            var match = await _unitOfWork.Matches.GetByIdAsync(id);
            if (match == null) return Result.Failure(new[] { "Match not found." });

            if (match.IsFinished)
            {
                var homeTeamEntry = await _unitOfWork.LeagueEntries.FindAsync(match.HomeTeamId, match.LeagueId);
                var awayTeamEntry = await _unitOfWork.LeagueEntries.FindAsync(match.AwayTeamId, match.LeagueId);
                if (homeTeamEntry != null && awayTeamEntry != null)
                {
                    RevertStats(homeTeamEntry, match.HomeTeamGoals.Value, match.AwayTeamGoals.Value);
                    RevertStats(awayTeamEntry, match.AwayTeamGoals.Value, match.HomeTeamGoals.Value);
                }
            }

            _unitOfWork.Matches.Delete(match);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        // --- Helper methods for applying and reverting stats ---
        private void ApplyStats(LeagueEntry entry, int goalsFor, int goalsAgainst)
        {
            entry.Played++;
            entry.GoalsFor += goalsFor;
            entry.GoalsAgainst += goalsAgainst;

            if (goalsFor > goalsAgainst) entry.Wins++;
            else if (goalsFor < goalsAgainst) entry.Losses++;
            else entry.Draws++;
        }

        private void RevertStats(LeagueEntry entry, int goalsFor, int goalsAgainst)
        {
            entry.Played = Math.Max(0, entry.Played - 1);
            entry.GoalsFor = Math.Max(0, entry.GoalsFor - goalsFor);
            entry.GoalsAgainst = Math.Max(0, entry.GoalsAgainst - goalsAgainst);

            if (goalsFor > goalsAgainst) entry.Wins = Math.Max(0, entry.Wins - 1);
            else if (goalsFor < goalsAgainst) entry.Losses = Math.Max(0, entry.Losses - 1);
            else entry.Draws = Math.Max(0, entry.Draws - 1);
        }

        // --- Getter Methods ---
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
            return await _unitOfWork.Matches.GetMatchWithTeamsByIdAsync(id);
        }

        public async Task<IEnumerable<Match>> GetAllMatchesWithDetailsAsync()
        {
            return await _unitOfWork.Matches.GetAllMatchesWithDetailsAsync();
        }
    }
}