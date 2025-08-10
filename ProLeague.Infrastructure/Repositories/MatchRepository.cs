using Microsoft.EntityFrameworkCore;
using ProLeague.Application.Interfaces;
using ProLeague.Domain.Entities;
using ProLeague.Infrastructure.Data;
using ProLeague.Infrastructure.Repositories;

public class MatchRepository : Repository<Match>, IMatchRepository
{
    public MatchRepository(ApplicationDbContext context) : base(context) { }
    public async Task<Match?> GetMatchWithTeamsByIdAsync(int id)
    {
        return await _context.Matches
            .Include(m => m.HomeTeam) // Load the Home Team
            .Include(m => m.AwayTeam) // Load the Away Team
            .FirstOrDefaultAsync(m => m.Id == id);
    }
    public async Task<IEnumerable<Match>> GetMatchesByWeekAsync(int leagueId, int week)
    {
        return await _context.Matches
            .Where(m => m.LeagueId == leagueId && m.MatchWeek == week)
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .OrderBy(m => m.MatchDate)
            .ToListAsync();
    }
    public async Task<IEnumerable<Match>> GetAllMatchesWithDetailsAsync()
    {
        return await _context.Matches
            .Include(m => m.HomeTeam)   // Load the related Home Team entity
            .Include(m => m.AwayTeam)   // Load the related Away Team entity
            .Include(m => m.League)     // Load the related League entity
            .OrderByDescending(m => m.MatchDate)
            .ToListAsync();
    }
    public async Task<IEnumerable<Match>> GetMatchesForTeamAsync(int teamId)
    {
        return await _context.Matches
            .Where(m => m.HomeTeamId == teamId || m.AwayTeamId == teamId)
            .Include(m => m.HomeTeam)
            .Include(m => m.AwayTeam)
            .OrderByDescending(m => m.MatchDate)
            .ToListAsync();
    }
}