// ProLeague.Application/Services/TeamService.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.Team;
using ProLeague.Domain.Entities;
using System.IO;

namespace ProLeague.Application.Services
{
    public class TeamService : ITeamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;

        public TeamService(IUnitOfWork unitOfWork, IWebHostEnvironment environment)
        {
            _unitOfWork = unitOfWork;
            _environment = environment;
        }

        public async Task<IEnumerable<Team>> GetAllTeamsAsync()
        {
            return await _unitOfWork.Teams.GetAllAsync();
        }

        public async Task<Team?> GetTeamDetailsAsync(int id)
        {
            return await _unitOfWork.Teams.GetTeamDetailsAsync(id);
        }

        public async Task<EditTeamViewModel?> GetTeamForEditAsync(int id)
        {
            var team = await _unitOfWork.Teams.GetByIdAsync(id);
            if (team == null) return null;

            return new EditTeamViewModel
            {
                Id = team.Id,
                Name = team.Name,
                Stadium = team.Stadium,
                LeagueId = team.LeagueId,
                ExistingLogoPath = team.ImagePath
            };
        }

        public async Task<Result> CreateTeamAsync(CreateTeamViewModel model)
        {
            var logoPath = await UploadFileAsync(model.LogoFile, "teams");
            var team = new Team
            {
                Name = model.Name,
                Stadium = model.Stadium,
                LeagueId = model.LeagueId,
                ImagePath = logoPath
            };

            await _unitOfWork.Teams.AddAsync(team);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        public async Task<Result> UpdateTeamAsync(EditTeamViewModel model)
        {
            var team = await _unitOfWork.Teams.GetByIdAsync(model.Id);
            if (team == null) return Result.Failure(new[] { "تیم یافت نشد." });

            if (model.NewLogoFile != null)
            {
                DeleteFile(team.ImagePath);
                team.ImagePath = await UploadFileAsync(model.NewLogoFile, "teams");
            }

            team.Name = model.Name;
            team.Stadium = model.Stadium;
            team.LeagueId = model.LeagueId;

            _unitOfWork.Teams.Update(team);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        public async Task<Result> DeleteTeamAsync(int id)
        {
            var team = await _unitOfWork.Teams.GetByIdAsync(id);
            if (team == null) return Result.Failure(new[] { "تیم یافت نشد." });

            DeleteFile(team.ImagePath);
            _unitOfWork.Teams.Delete(team);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        private async Task<string?> UploadFileAsync(IFormFile? file, string subfolder)
        {
            if (file == null || file.Length == 0)
                return null;

            // مسیر فولدر ذخیره‌سازی
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", subfolder);

            // ایجاد پوشه اگر وجود ندارد
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // نام یکتا برای فایل
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // ذخیره فایل روی دیسک
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // مسیر نسبی برای ذخیره در دیتابیس
            return $"/images/{subfolder}/{uniqueFileName}";
        }

        private void DeleteFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath))
                return;

            // ساخت مسیر فیزیکی از مسیر نسبی
            var fullPath = Path.Combine(_environment.WebRootPath, relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
