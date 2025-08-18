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

        public async Task<Team?> GetTeamDetailsAsync(int id)
        {
            // This method is for the public details page
            return await _context.Teams
                .Include(t => t.LeagueEntries).ThenInclude(le => le.League)
                .Include(t => t.Players)
                .FirstOrDefaultAsync(t => t.Id == id);
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