using Microsoft.Extensions.DependencyInjection; // این using را اضافه کنید
using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.Admin;
using System.Threading.Tasks;

namespace ProLeague.Application.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AdminDashboardService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task<AdminDashboardViewModel> GetDashboardDataAsync()
        {
            // هر وظیفه در یک scope جداگانه اجرا می‌شود تا UnitOfWork و DbContext مجزا داشته باشد
            var leaguesCountTask = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                return await unitOfWork.Leagues.CountAsync();
            });

            var teamsCountTask = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                return await unitOfWork.Teams.CountAsync();
            });

            var playersCountTask = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                return await unitOfWork.Players.CountAsync();
            });

            var newsCountTask = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                return await unitOfWork.News.CountAsync();
            });

            var commentsCountTask = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                return await unitOfWork.Comments.CountAsync();
            });

            var recentNewsTask = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                return await unitOfWork.News.GetRecentNewsAsync(5);
            });

            var recentCommentsTask = Task.Run(async () =>
            {
                using var scope = _scopeFactory.CreateScope();
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                return await unitOfWork.Comments.GetRecentCommentsAsync(5);
            });

            // منتظر بمانید تا تمام وظایف موازی به پایان برسند
            await Task.WhenAll(
                leaguesCountTask,
                teamsCountTask,
                playersCountTask,
                newsCountTask,
                commentsCountTask,
                recentNewsTask,
                recentCommentsTask);

            // نتایج را جمع‌آوری کنید
            return new AdminDashboardViewModel
            {
                TotalLeagues = await leaguesCountTask,
                TotalTeams = await teamsCountTask,
                TotalPlayers = await playersCountTask,
                TotalNews = await newsCountTask,
                TotalComments = await commentsCountTask,
                RecentNews = await recentNewsTask,
                RecentComments = await recentCommentsTask
            };
        }
    }
}