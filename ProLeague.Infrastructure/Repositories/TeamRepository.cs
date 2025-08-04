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
        public TeamRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Team?> GetTeamDetailsAsync(int id)
        {
            return await _context.Teams
                .Include(t => t.League)
                .Include(t => t.Players)
                .Include(t => t.HomeMatches)
                .Include(t => t.AwayMatches)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }
}