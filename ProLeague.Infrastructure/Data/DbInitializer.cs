// ProLeague.Infrastructure/Data/DbInitializer.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ProLeague.Domain.Constants;
using ProLeague.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace ProLeague.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            // 1. ایجاد نقش‌ها (Roles)
            string[] roleNames = { RoleNames.Admin, RoleNames.Editor, RoleNames.User };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 2. ایجاد کاربر ادمین
            string adminEmail = "admin@proleague.com";
            string adminPassword = "Admin@123";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, RoleNames.Admin);
                    Console.WriteLine("Admin user created and assigned to Admin role.");
                }
            }
        }
    }
}