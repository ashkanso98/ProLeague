// ProLeague.Application/Services/LeagueService.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.League;
using ProLeague.Domain.Entities;
using System.IO;

namespace ProLeague.Application.Services
{
    public class LeagueService : ILeagueService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;

        public LeagueService(IUnitOfWork unitOfWork, IWebHostEnvironment environment)
        {
            _unitOfWork = unitOfWork;
            _environment = environment;
        }
        public async Task<IEnumerable<League>> GetAllLeaguesWithTeamsAsync()
        {
            return await _unitOfWork.Leagues.GetAllLeaguesWithTeamsAsync();
        }
        public async Task<League?> GetLeagueDetailsAsync(int id)
        {
            return await _unitOfWork.Leagues.GetLeagueDetailsAsync(id);
        }

        public async Task<IEnumerable<League>> GetAllLeaguesAsync()
        {
            return await _unitOfWork.Leagues.GetAllAsync();
        }

        public async Task<League?> GetLeagueByIdAsync(int id)
        {
            return await _unitOfWork.Leagues.GetByIdAsync(id);
        }

        public async Task<EditLeagueViewModel?> GetLeagueForEditAsync(int id)
        {
            var league = await _unitOfWork.Leagues.GetByIdAsync(id);
            if (league == null) return null;

            return new EditLeagueViewModel
            {
                Id = league.Id,
                Name = league.Name,
                Country = league.Country,
                ExistingLogoPath = league.ImagePath
            };
        }

        public async Task<Result> CreateLeagueAsync(CreateLeagueViewModel model)
        {
            try
            {
                var logoPath = await UploadFileAsync(model.LogoFile, "league");
                var league = new League
                {
                    Name = model.Name,
                    Country = model.Country,
                    ImagePath = logoPath
                };

                await _unitOfWork.Leagues.AddAsync(league);
                await _unitOfWork.CompleteAsync();
                return Result.Success();
            }
            catch (Exception ex)
            {
                // Log the exception (ex)
                return Result.Failure(new[] { "خطایی در هنگام ایجاد لیگ رخ داد." });
            }
        }

        public async Task<Result> UpdateLeagueAsync(EditLeagueViewModel model)
        {
            var league = await _unitOfWork.Leagues.GetByIdAsync(model.Id);
            if (league == null) return Result.Failure(new[] { "لیگ مورد نظر یافت نشد." });

            if (model.NewLogoFile != null)
            {
                DeleteFile(league.ImagePath);
                league.ImagePath = await UploadFileAsync(model.NewLogoFile, "league");
            }

            league.Name = model.Name;
            league.Country = model.Country;

            _unitOfWork.Leagues.Update(league);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        public async Task<Result> DeleteLeagueAsync(int id)
        {
            var league = await _unitOfWork.Leagues.GetByIdAsync(id);
            if (league == null) return Result.Failure(new[] { "لیگ مورد نظر یافت نشد." });

            DeleteFile(league.ImagePath);
            _unitOfWork.Leagues.Delete(league);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        private async Task<string?> UploadFileAsync(IFormFile? file, string subfolder)
        {
            if (file == null || file.Length == 0) return null;
            string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", subfolder);
            Directory.CreateDirectory(uploadsFolder);
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return $"/images/{subfolder}/{uniqueFileName}";
        }

        private void DeleteFile(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;
            string filePath = Path.Combine(_environment.WebRootPath, relativePath.TrimStart('/'));
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public async Task<IEnumerable<League>> GetAllLeaguesWithTeamsAsync(string season)
        {
            return await _unitOfWork.Leagues.GetAllLeaguesWithTeamsAsync(season);
        }

        public async Task<League?> GetLeagueDetailsAsync(int id, string season)
        {
            return await _unitOfWork.Leagues.GetLeagueDetailsAsync(id, season);
        }
    }
}