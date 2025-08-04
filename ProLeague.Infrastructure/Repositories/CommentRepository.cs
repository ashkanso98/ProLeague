// ProLeague.Infrastructure/Repositories/CommentRepository.cs
using Microsoft.EntityFrameworkCore;
using ProLeague.Application.Interfaces;
using ProLeague.Domain.Entities;
using ProLeague.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProLeague.Infrastructure.Repositories
{
    public class CommentRepository : Repository<NewsComment>, ICommentRepository
    {
        public CommentRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<NewsComment>> GetCommentsByStatusAsync(CommentStatus status)
        {
            return await _context.NewsComments
                .Where(c => c.Status == status)
                .Include(c => c.User)
                .Include(c => c.News)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }
    }
}