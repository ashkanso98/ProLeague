// ProLeague.Infrastructure/Repositories/UserRepository.cs
// این فایل جدید را بسازید
using Microsoft.EntityFrameworkCore;
using ProLeague.Application.Interfaces;
using ProLeague.Domain.Entities;
using ProLeague.Infrastructure.Data;

namespace ProLeague.Infrastructure.Repositories
{
    public class UserRepository : Repository<ApplicationUser>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context) { }

        public async Task<ApplicationUser?> GetUserWithFavoriteTeamAsync(string id)
        {
            return await _context.Users
                .Include(u => u.FavoriteTeam)
                .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}