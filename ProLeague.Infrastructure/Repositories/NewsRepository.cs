// ProLeague.Infrastructure/Repositories/NewsRepository.cs
using Microsoft.EntityFrameworkCore;
using ProLeague.Application;
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
                .Include(n => n.Comments)  // اضافه کردن این خط برای بارگذاری نظرات
                .ThenInclude(c => c.User)  // برای بارگذاری User (چون در View از User.UserName استفاده می‌شود)
                .FirstOrDefaultAsync(n => n.Id == id);
        }
        public async Task<PaginatedList<News>> GetPaginatedFilteredNewsAsync(int? leagueId, int? teamId, int pageIndex, int pageSize)
        {
            var query = _context.News.AsNoTracking();

            if (leagueId.HasValue)
            {
                query = query.Where(n => n.RelatedLeagues.Any(l => l.Id == leagueId.Value));
            }

            if (teamId.HasValue)
            {
                query = query.Where(n => n.RelatedTeams.Any(t => t.Id == teamId.Value));
            }

            query = query.OrderByDescending(n => n.PublishedDate);

            return await PaginatedList<News>.CreateAsync(query, pageIndex, pageSize);
        }
        public async Task<NewsImage?> GetNewsImageByIdAsync(int id)
        {
            return await _context.NewsImages.FindAsync(id);
        }

        public void DeleteNewsImage(NewsImage image)
        {
            _context.NewsImages.Remove(image);
        }
        public async Task<IEnumerable<News>> GetRecentNewsAsync(int count)
        {
            return await _context.News
                .OrderByDescending(n => n.PublishedDate)
                .Take(count)
                .ToListAsync();
        }
    }
}