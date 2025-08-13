// ProLeague.Infrastructure/Repositories/LeagueRepository.cs
using Microsoft.EntityFrameworkCore;
using ProLeague.Application.Interfaces;
using ProLeague.Domain.Entities;
using ProLeague.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProLeague.Infrastructure.Repositories
{
    public class LeagueRepository : Repository<League>, ILeagueRepository
    {
        public LeagueRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<List<League>> GetAllLeaguesWithTeamsAsync()
        {
            return await _context.Leagues
                .Include(l => l.Teams)
                .OrderBy(l => l.Name)
                .ToListAsync();
        }
        public async Task<List<League>> GetTopLeaguesAsync(int count, int excludeId)
        {
            return await _context.Leagues
                .Where(l => l.Id != excludeId) // لیگ پین‌شده را نادیده بگیر
                .Include(l => l.Teams)
                .Take(count)
                .ToListAsync();
        }
        public async Task<IEnumerable<League>> GetLeaguesWithTeamsAsync(int count)
        {
            return await _context.Leagues
                .Include(l => l.Teams)
                .Take(count)
                .ToListAsync();
        }

        // متد کامل برای دریافت جزئیات لیگ
        public async Task<League?> GetLeagueDetailsAsync(int id)
        {
            return await _context.Leagues
                // ۱. تیم‌های حاضر در لیگ را بارگذاری کن
                .Include(l => l.Teams)
                    // ۲. برای هر تیم، بازی‌های خانگی آن را بارگذاری کن (برای محاسبه امتیاز)
                    .ThenInclude(t => t.HomeMatches)
                .Include(l => l.Teams)
                    // ۳. برای هر تیم، بازی‌های خارج از خانه را نیز بارگذاری کن
                    .ThenInclude(t => t.AwayMatches)

                // ۴. تمام مسابقات برگزار شده در این لیگ را بارگذاری کن
                .Include(l => l.Matches)
                    // ۵. برای هر مسابقه، اطلاعات تیم میزبان را بارگذاری کن
                    .ThenInclude(m => m.HomeTeam)
                .Include(l => l.Matches)
                    // ۶. برای هر مسابقه، اطلاعات تیم میهمان را نیز بارگذاری کن
                    .ThenInclude(m => m.AwayTeam)

                // بهینه‌سازی: چون فقط قصد نمایش اطلاعات را داریم، نیازی به ردگیری تغییرات نیست
                .AsNoTracking()

                // لیگ مورد نظر را بر اساس شناسه پیدا کن
                .FirstOrDefaultAsync(l => l.Id == id);
        }
        public async Task<List<League>> GetHomepageLeaguesAsync(int count, int pinnedLeagueId)
        {
            // 1. Fetch the top 'count' leagues, making sure to exclude the pinned one for now.
            var topLeagues = await _context.Leagues
                .Where(l => l.Id != pinnedLeagueId)
                .Include(l => l.Teams)
                .Take(count)
                .ToListAsync();

            // 2. Fetch the pinned league separately to ensure it's included.
            var pinnedLeague = await _context.Leagues
                .Where(l => l.Id == pinnedLeagueId)
                .Include(l => l.Teams)
                .FirstOrDefaultAsync();

            // 3. Add the pinned league to the beginning of the list if it exists.
            if (pinnedLeague != null)
            {
                // Ensure we don't add duplicates if the pinned league was somehow in the top list
                topLeagues.RemoveAll(l => l.Id == pinnedLeagueId);
                topLeagues.Insert(0, pinnedLeague);
            }

            return topLeagues;
        }
    }
}