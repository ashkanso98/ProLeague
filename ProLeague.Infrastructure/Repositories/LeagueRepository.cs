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

        public async Task<IEnumerable<League>> GetLeaguesWithTeamsAsync(int count)
        {
            return await _context.Leagues
                // Include the join entity, then the Team through it
                .Include(l => l.TeamEntries).ThenInclude(le => le.Team)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<League>> GetAllLeaguesWithTeamsAsync()
        {
            return await _context.Leagues
                .Include(l => l.TeamEntries).ThenInclude(le => le.Team)
                .OrderBy(l => l.Name)
                .ToListAsync();
        }

        public async Task<List<League>> GetTopLeaguesAsync(int count, int excludeId)
        {
            return await _context.Leagues
                .Where(l => l.Id != excludeId)
                .Include(l => l.TeamEntries).ThenInclude(le => le.Team)
                .Take(count)
                .ToListAsync();
        }

        public async Task<League?> GetLeagueDetailsAsync(int id)
        {
            return await _context.Leagues
                // Include the TeamEntries and then the actual Team for each entry
                .Include(l => l.TeamEntries).ThenInclude(le => le.Team)
                // Also load the matches for each team entry to calculate stats
                .Include(l => l.TeamEntries).ThenInclude(le => le.Team.HomeMatches)
                .Include(l => l.TeamEntries).ThenInclude(le => le.Team.AwayMatches)
                // Load the league's overall matches and their teams
                .Include(l => l.Matches).ThenInclude(m => m.HomeTeam)
                .Include(l => l.Matches).ThenInclude(m => m.AwayTeam)
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<List<League>> GetHomepageLeaguesAsync(int count, int pinnedLeagueId)
        {
            // This method needs to be updated to handle the new structure as well.
            // For now, let's simplify it to get the pinned league and top others.
            // A more complex query might be needed later.

            var pinnedLeague = await _context.Leagues
                .Where(l => l.Id == pinnedLeagueId)
                .Include(l => l.TeamEntries).ThenInclude(le => le.Team)
                .FirstOrDefaultAsync();

            var topLeagues = await _context.Leagues
                .Where(l => l.Id != pinnedLeagueId)
                .Include(l => l.TeamEntries).ThenInclude(le => le.Team)
                .OrderByDescending(l => l.Id) // Just an example order
                .Take(count)
                .ToListAsync();

            if (pinnedLeague != null)
            {
                topLeagues.Insert(0, pinnedLeague);
            }

            return topLeagues.DistinctBy(l => l.Id).ToList();
        }
    }
}