// FootballNews.Web/Program.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProLeague.Domain.Constants;
using ProLeague.Domain.Entities;
using ProLeague.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// فقط یکی از این دو را استفاده کنید. AddIdentity برای پشتیبانی کامل از Roles ترجیح داده می‌شود.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    // می‌توانید گزینه‌های دیگر را نیز تنظیم کنید
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddRoles<IdentityRole>(); // برای فعال کردن RoleManager

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // برای صفحات Identity

var app = builder.Build();

// ایجاد Roles و کاربر ادمین
await CreateRolesAndAdminUser(app.Services);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


//***************

// === بخش جدید برای ایجاد کاربر ادمین ===
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;
    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // 1. اطمینان از وجود Role "Admin"
    string adminRoleName = "Admin"; // یا RoleNames.Admin اگر از کلاس ثابت استفاده می‌کنید
    if (!await roleManager.RoleExistsAsync(adminRoleName))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRoleName));
    }

    // 2. اطلاعات کاربر ادمین
    string adminEmail = "admin@proleague.com"; // ایمیل کاربر ادمین
    string adminPassword = "Admin@123";       // رمز عبور کاربر ادمین (حتماً تغییر دهید!)

    // 3. بررسی وجود کاربر
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        // 4. ایجاد کاربر جدید
        adminUser = new ApplicationUser
        {
            UserName = adminEmail, // معمولاً UserName همان Email است
            Email = adminEmail,
            EmailConfirmed = true // فرض: ایمیل تأیید شده است. در محیط واقعی بهتر است تأیید ایمیل را پیاده‌سازی کنید.
            // سایر فیلدهای سفارشی ApplicationUser را در صورت نیاز پر کنید
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);

        if (result.Succeeded)
        {
            // 5. دادن Role "Admin" به کاربر ایجاد شده
            await userManager.AddToRoleAsync(adminUser, adminRoleName);
            Console.WriteLine($"کاربر ادمین '{adminEmail}' با موفقیت ایجاد و به Role '{adminRoleName}' اختصاص یافت.");
        }
        else
        {
            Console.WriteLine("خطا در ایجاد کاربر ادمین:");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"- {error.Description}");
            }
        }
    }
    else
    {
        // اگر کاربر وجود داشت، بررسی کنید آیا Role ادمین دارد یا نه
        if (!await userManager.IsInRoleAsync(adminUser, adminRoleName))
        {
            await userManager.AddToRoleAsync(adminUser, adminRoleName);
            Console.WriteLine($"کاربر '{adminEmail}' پیدا شد و Role '{adminRoleName}' به او اختصاص یافت.");
        }
        else
        {
            Console.WriteLine($"کاربر ادمین '{adminEmail}' از قبل وجود دارد.");
        }
    }
}
// === پایان بخش جدید ===

// ... (بقیه کدهای قبلی Program.cs)


//*****************



app.UseHttpsRedirection();
app.UseStaticFiles(); // For CSS, JS, Images

app.UseRouting();

app.UseAuthentication(); // Add Authentication
app.UseAuthorization();  // Add Authorization

app.MapControllerRoute(
    name: "AdminArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
// ... سایر Routeها ...
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages(); // For Identity pages

app.Run();

// متد جدا برای ایجاد Roles و کاربر ادمین
async Task CreateRolesAndAdminUser(IServiceProvider serviceProvider)
{
    using (var scope = serviceProvider.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // ایجاد Roles اگر وجود ندارند
        string[] roleNames = { RoleNames.Admin, RoleNames.User, RoleNames.Editor };
        IdentityResult roleResult;

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        // ایجاد کاربر Admin اولیه (اختیاری)
        var adminEmail = "admin@proleague.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var newAdminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var createAdminUser = await userManager.CreateAsync(newAdminUser, "Admin@123");
            if (createAdminUser.Succeeded)
            {
                // دادن Role Admin به کاربر
                await userManager.AddToRoleAsync(newAdminUser, RoleNames.Admin);
            }
        }
    }
}