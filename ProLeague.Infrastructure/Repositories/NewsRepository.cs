// ProLeague.Infrastructure/Repositories/NewsRepository.cs
using Microsoft.EntityFrameworkCore;
using ProLeague.Application.Interfaces;
using ProLeague.Domain.Entities;
using ProLeague.Infrastructure.Data;
using System.Threading.Tasks;

namespace ProLeague.Infrastructure.Repositories
{
    public class NewsRepository : Repository<News>, INewsRepository
    {
        public NewsRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<News?> GetNewsDetailsAsync(int id)
        {
            return await _context.News
                .Include(n => n.Images)
                .Include(n => n.RelatedLeagues)
                .Include(n => n.RelatedTeams)
                .Include(n => n.RelatedPlayers)
                .FirstOrDefaultAsync(n => n.Id == id);
        }

        public async Task<NewsImage?> GetNewsImageByIdAsync(int id)
        {
            return await _context.NewsImages.FindAsync(id);
        }

        public void DeleteNewsImage(NewsImage image)
        {
            _context.NewsImages.Remove(image);
        }
    }
}