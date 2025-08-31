// ProLeague/Program.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using ProLeague.Application.Interfaces;
using ProLeague.Application.Services;
using ProLeague.Domain.Entities;
using ProLeague.Infrastructure.Data;
using ProLeague.Infrastructure.Repositories;
using ProLeague.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------------------
// 1. ثبت سرویس‌های اصلی (Dependency Injection)
// ----------------------------------------------------------------

// اتصال به دیتابیس
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// تنظیمات Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
// ثبت UnitOfWork و تمام سرویس‌های لایه Application
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ILeagueService, LeagueService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<INewsService, NewsService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IHomeService, HomeService>(); 
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAdminDashboardService, AdminDashboardService>();
builder.Services.AddScoped<IPointDeductionService, PointDeductionService>();

//builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
// سرویس‌های وب
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // برای صفحات Identity

// ----------------------------------------------------------------
// 2. ساخت اپلیکیشن
// ----------------------------------------------------------------
var app = builder.Build();

// اجرای فرآیند Seed کردن دیتابیس (ایجاد نقش‌ها و کاربر ادمین)
using (var scope = app.Services.CreateScope())
{
    await DbInitializer.Initialize(scope.ServiceProvider);
}

// ----------------------------------------------------------------
// 3. پیکربندی Pipeline درخواست‌های HTTP
// ----------------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// تعریف Route ها
app.MapControllerRoute(
    name: "AdminArea",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// ----------------------------------------------------------------
// 4. اجرای برنامه
// ----------------------------------------------------------------
app.Run();