using Microsoft.EntityFrameworkCore;
using ProLeague.Application.Interfaces;
using ProLeague.Domain.Entities;
using ProLeague.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProLeague.Infrastructure.Repositories
{
    public class PointDeductionRepository : Repository<PointDeduction>, IPointDeductionRepository
    {
        public PointDeductionRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<PointDeduction>> GetDeductionsByLeagueAsync(int leagueId)
        {
            return await _context.PointDeductions
                .Where(d => d.LeagueId == leagueId)
                // Corrected: Include LeagueEntry first, then include the Team from it.
                .Include(d => d.LeagueEntry)
                .ThenInclude(le => le.Team)
                .OrderByDescending(d => d.DateApplied)
                .ToListAsync();
        }

        public async Task<PointDeduction?> GetDeductionDetailsAsync(int id)
        {
            return await _context.PointDeductions
                // Corrected: Include LeagueEntry first, then include Team and League from it.
                .Include(d => d.LeagueEntry)
                .ThenInclude(le => le.Team)
                .Include(d => d.LeagueEntry)
                .ThenInclude(le => le.League)
                .FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}