// ProLeague/Controllers/UserManagementController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.User;

namespace ProLeague.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly IUserService _userService;

        public UserManagementController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: /UserManagement
        public async Task<IActionResult> Index()
        {
            var usersWithRoles = await _userService.GetAllUsersWithRolesAsync();
            return View(usersWithRoles);
        }

        // GET: /UserManagement/ManageRoles/{userId}
        public async Task<IActionResult> ManageRoles(string userId)
        {
            var model = await _userService.GetUserForManagingRolesAsync(userId);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: /UserManagement/ManageRoles
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> ManageRoles(ManageUserRolesViewModel model)
        //{
        //    var result = await _userService.UpdateUserRolesAsync(model.UserId, model.UserRoles.ToList());

        //    if (result.Succeeded)
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }

        //    foreach (var error in result.Errors)
        //    {
        //        ModelState.AddModelError(string.Empty, error);
        //    }

        //    // Repopulate the model for the view if there's an error
        //    var repopulatedModel = await _userService.GetUserForManagingRolesAsync(model.UserId);
        //    return View(repopulatedModel);
        //}


        // ProLeague/Controllers/UserManagementController.cs
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRoles(ManageUserRolesViewModel model)
        {
            // کد اشتباه قبلی:
            // var result = await _userService.UpdateUserRolesAsync(model.UserId, model.UserRoles.ToList());

            // کد صحیح:
            var result = await _userService.UpdateUserRolesAsync(model.UserId, model.SelectedRoles);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "نقش‌های کاربر با موفقیت به‌روزرسانی شد.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors ?? new List<string>())
            {
                ModelState.AddModelError(string.Empty, error);
            }

            // در صورت بروز خطا، مدل را برای نمایش مجدد آماده می‌کنیم
            var repopulatedModel = await _userService.GetUserForManagingRolesAsync(model.UserId);
            return View(repopulatedModel);
        }
    }
}