// ProLeague.Infrastructure/Repositories/PlayerRepository.cs
using Microsoft.EntityFrameworkCore;
using ProLeague.Application.Interfaces;
using ProLeague.Domain.Entities;
using ProLeague.Infrastructure.Data;

namespace ProLeague.Infrastructure.Repositories
{
    public class PlayerRepository : Repository<Player>, IPlayerRepository
    {
        public PlayerRepository(ApplicationDbContext context) : base(context) { }

        public async Task<Player?> GetPlayerWithTeamDetailsAsync(int id)
        {
            return await _context.Players
                .Include(p => p.Team) // اطلاعات تیم را بارگذاری می‌کند
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}