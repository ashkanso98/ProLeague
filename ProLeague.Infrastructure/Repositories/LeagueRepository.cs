using Microsoft.EntityFrameworkCore;
using ProLeague.Application.Interfaces;
using ProLeague.Domain.Entities;
using ProLeague.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProLeague.Infrastructure.Repositories
{
    public class LeagueRepository : Repository<League>, ILeagueRepository
    {
        public LeagueRepository(ApplicationDbContext context) : base(context)
        {
        }

        // --- This is the method you asked for, now complete ---
        public async Task<IEnumerable<League>> GetLeaguesWithTeamsAsync(int count)
        {
            return await _context.Leagues
                .Include(l => l.Teams)
                    .ThenInclude(t => t.HomeMatches)
                .Include(l => l.Teams)
                    .ThenInclude(t => t.AwayMatches)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<League>> GetAllLeaguesWithTeamsAsync()
        {
            return await _context.Leagues
                .Include(l => l.Teams).ThenInclude(t => t.HomeMatches)
                .Include(l => l.Teams).ThenInclude(t => t.AwayMatches)
                .OrderBy(l => l.Name)
                .ToListAsync();
        }

        public async Task<List<League>> GetTopLeaguesAsync(int count, int excludeId)
        {
            return await _context.Leagues
                .Where(l => l.Id != excludeId)
                .Include(l => l.Teams).ThenInclude(t => t.HomeMatches)
                .Include(l => l.Teams).ThenInclude(t => t.AwayMatches)
                .Take(count)
                .ToListAsync();
        }

        public async Task<League?> GetLeagueDetailsAsync(int id)
        {
            return await _context.Leagues
                .Include(l => l.Teams)
                    .ThenInclude(t => t.HomeMatches)
                .Include(l => l.Teams)
                    .ThenInclude(t => t.AwayMatches)
                .Include(l => l.Matches)
                    .ThenInclude(m => m.HomeTeam)
                .Include(l => l.Matches)
                    .ThenInclude(m => m.AwayTeam)
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<List<League>> GetHomepageLeaguesAsync(int count, int pinnedLeagueId)
        {
            var topLeagues = await _context.Leagues
                .Where(l => l.Id != pinnedLeagueId)
                .Include(l => l.Teams).ThenInclude(t => t.HomeMatches)
                .Include(l => l.Teams).ThenInclude(t => t.AwayMatches)
                .OrderByDescending(l => l.Id)
                .Take(count)
                .ToListAsync();

            var pinnedLeague = await _context.Leagues
                .Where(l => l.Id == pinnedLeagueId)
                .Include(l => l.Teams).ThenInclude(t => t.HomeMatches)
                .Include(l => l.Teams).ThenInclude(t => t.AwayMatches)
                .FirstOrDefaultAsync();

            if (pinnedLeague != null)
            {
                topLeagues.RemoveAll(l => l.Id == pinnedLeagueId);
                topLeagues.Insert(0, pinnedLeague);
            }

            return topLeagues;
        }
    }
}