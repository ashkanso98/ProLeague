// ProLeague.Infrastructure/Repositories/MatchRepository.cs
using Microsoft.EntityFrameworkCore;
using ProLeague.Application.Interfaces;
using ProLeague.Domain.Entities;
using ProLeague.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProLeague.Infrastructure.Repositories
{
    public class MatchRepository : Repository<Match>, IMatchRepository
    {
        public MatchRepository(ApplicationDbContext context) : base(context)
        {
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
    }
}