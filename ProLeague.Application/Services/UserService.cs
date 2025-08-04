// ProLeague.Application/Services/UserService.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.User;
using ProLeague.Domain.Entities;

namespace ProLeague.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUnitOfWork _unitOfWork;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<List<UserRolesViewModel>> GetAllUsersWithRolesAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRolesViewModel = new List<UserRolesViewModel>();

            foreach (var user in users)
            {
                userRolesViewModel.Add(new UserRolesViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = await _userManager.GetRolesAsync(user)
                });
            }
            return userRolesViewModel;
        }

        public async Task<ManageUserRolesViewModel?> GetUserForManagingRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var allRoles = await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
            var userRoles = await _userManager.GetRolesAsync(user);

            return new ManageUserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                AllRoles = allRoles,
                UserRoles = userRoles
            };
        }

        public async Task<Result> UpdateUserRolesAsync(string userId, List<string> selectedRoles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Result.Failure(new[] { "کاربر یافت نشد." });

            var currentRoles = await _userManager.GetRolesAsync(user);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);

            if (!removeResult.Succeeded)
            {
                return Result.Failure(removeResult.Errors.Select(e => e.Description));
            }

            if (selectedRoles != null && selectedRoles.Any())
            {
                var addResult = await _userManager.AddToRolesAsync(user, selectedRoles);
                if (!addResult.Succeeded)
                {
                    // اگر افزودن نقش جدید با خطا مواجه شد، بهتر است نقش‌های قبلی را برگردانیم
                    await _userManager.AddToRolesAsync(user, currentRoles);
                    return Result.Failure(addResult.Errors.Select(e => e.Description));
                }
            }

            return Result.Success();
        }

        //public async Task<ApplicationUser?> GetUserWithFavoriteTeamAsync(string userId)
        //{
        //    // UserManager از Include پشتیبانی نمی‌کند، پس کاربر را با DbContext می‌خوانیم
        //    return await _unitOfWork.Users.Users // فرض: IUnitOfWork یک پراپرتی برای Users دارد
        //        .Include(u => u.FavoriteTeam)
        //        .FirstOrDefaultAsync(u => u.Id == userId);
        //}
        // ProLeague.Application/Services/UserService.cs
        public async Task<ApplicationUser?> GetUserWithFavoriteTeamAsync(string userId)
        {
            return await _unitOfWork.Users.GetUserWithFavoriteTeamAsync(userId);
        }

        public async Task<Result> SetFavoriteTeamAsync(string userId, int? teamId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Result.Failure(new[] { "کاربر یافت نشد." });

            var oldFavoriteTeamId = user.FavoriteTeamId;

            // اگر تیم جدید انتخاب شده، بررسی می‌کنیم که وجود داشته باشد
            if (teamId.HasValue)
            {
                var teamExists = await _unitOfWork.Teams.GetByIdAsync(teamId.Value);
                if (teamExists == null)
                {
                    return Result.Failure(new[] { "تیم انتخاب شده معتبر نیست." });
                }
            }

            // بروزرسانی تیم محبوب کاربر
            user.FavoriteTeamId = teamId;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                return Result.Failure(updateResult.Errors.Select(e => e.Description));
            }

            // بروزرسانی تعداد طرفداران تیم‌ها
            // این منطق پیچیده اکنون در لایه Application قرار دارد و کنترلر از آن بی‌خبر است
            if (oldFavoriteTeamId.HasValue)
            {
                var oldTeam = await _unitOfWork.Teams.GetByIdAsync(oldFavoriteTeamId.Value);
                if (oldTeam != null)
                {
                    oldTeam.FanCount = Math.Max(0, oldTeam.FanCount - 1);
                }
            }

            if (teamId.HasValue)
            {
                var newTeam = await _unitOfWork.Teams.GetByIdAsync(teamId.Value);
                if (newTeam != null)
                {
                    newTeam.FanCount++;
                }
            }

            await _unitOfWork.CompleteAsync(); // ذخیره تغییرات FanCount

            return Result.Success();
        }
    }
}