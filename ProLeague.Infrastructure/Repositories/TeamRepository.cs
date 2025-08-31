// ProLeague.Infrastructure/Repositories/TeamRepository.cs
using Microsoft.EntityFrameworkCore;
using ProLeague.Application.Interfaces;
using ProLeague.Domain.Entities;
using ProLeague.Infrastructure.Data;
using System.Threading.Tasks;

namespace ProLeague.Infrastructure.Repositories
{
    public class TeamRepository : Repository<Team>, ITeamRepository
    {
        public TeamRepository(ApplicationDbContext context) : base(context) { }


        //public async Task<Team?> GetTeamDetailsAsync(int id)
        //{
        //    return await _context.Teams
        //        // Load the leagues the team is in
        //        .Include(t => t.LeagueEntries)
        //        .ThenInclude(le => le.League)
        //        // Load the players of the team
        //        .Include(t => t.Players)
        //        // Load the matches where this team was the home team
        //        .Include(t => t.HomeMatches)
        //        .ThenInclude(m => m.AwayTeam) // For each home match, get the opponent
        //        // Load the matches where this team was the away team
        //        .Include(t => t.AwayMatches)
        //        .ThenInclude(m => m.HomeTeam) // For each away match, get the opponent
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync(t => t.Id == id);
        //}
        public async Task<Team?> GetTeamDetailsAsync(int id)
        {
            return await _context.Teams
                .Include(t => t.LeagueEntries).ThenInclude(le => le.League)
                .Include(t => t.Players)
                // Load home matches and their related league
                .Include(t => t.HomeMatches).ThenInclude(m => m.League) // <-- ADD THIS
                .Include(t => t.HomeMatches).ThenInclude(m => m.AwayTeam)
                // Load away matches and their related league
                .Include(t => t.AwayMatches).ThenInclude(m => m.League) // <-- ADD THIS
                .Include(t => t.AwayMatches).ThenInclude(m => m.HomeTeam)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);
        }
        public async Task<IEnumerable<Team>> GetAllTeamsWithLeaguesAsync()
        {
            return await _context.Teams
                .Include(t => t.LeagueEntries)
                .ThenInclude(le => le.League)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }
        public async Task<IEnumerable<Team>> GetTeamsByLeagueIdAsync(int leagueId)
        {
            return await _context.Teams
                .Where(t => t.LeagueEntries.Any(le => le.LeagueId == leagueId))
                .OrderBy(t => t.Name)
                .ToListAsync();
        }
        // New method implementation
        public async Task<Team?> GetTeamWithLeaguesAsync(int id)
        {
            return await _context.Teams
                .Include(t => t.LeagueEntries) // Include the join entity
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}