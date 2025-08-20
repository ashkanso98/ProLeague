using ProLeague.Application.Interfaces;
using ProLeague.Domain.Entities;
using ProLeague.Infrastructure.Data;
using System.Threading.Tasks;

namespace ProLeague.Infrastructure.Repositories
{
    public class LeagueEntryRepository : Repository<LeagueEntry>, ILeagueEntryRepository
    {
        public LeagueEntryRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<LeagueEntry?> FindAsync(int teamId, int leagueId)
        {
            return await _context.LeagueEntries.FindAsync(teamId, leagueId);
        }
    }
}