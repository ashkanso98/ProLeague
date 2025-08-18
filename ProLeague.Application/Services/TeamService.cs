using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.Team;
using ProLeague.Domain.Entities;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
        public async Task<IEnumerable<Team>> GetTeamsByLeagueIdAsync(int leagueId)
        {
            return await _unitOfWork.Teams.GetTeamsByLeagueIdAsync(leagueId);
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
            var team = await _unitOfWork.Teams.GetTeamWithLeaguesAsync(id);
            if (team == null) return null;

            return new EditTeamViewModel
            {
                Id = team.Id,
                Name = team.Name,
                Stadium = team.Stadium,
                ExistingLogoPath = team.ImagePath,
                // Map the list of league IDs from the join entity
                LeagueIds = team.LeagueEntries.Select(le => le.LeagueId).ToList()
            };
        }

        public async Task<Result> CreateTeamAsync(CreateTeamViewModel model)
        {
            var logoPath = await UploadFileAsync(model.LogoFile, "teams");
            var team = new Team
            {
                Name = model.Name,
                Stadium = model.Stadium,
                ImagePath = logoPath
            };

            // Create a LeagueEntry for each selected league
            if (model.LeagueIds != null && model.LeagueIds.Any())
            {
                foreach (var leagueId in model.LeagueIds)
                {
                    team.LeagueEntries.Add(new LeagueEntry { LeagueId = leagueId });
                }
            }

            await _unitOfWork.Teams.AddAsync(team);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        public async Task<Result> UpdateTeamAsync(EditTeamViewModel model)
        {
            var teamToUpdate = await _unitOfWork.Teams.GetTeamWithLeaguesAsync(model.Id);
            if (teamToUpdate == null) return Result.Failure(new[] { "تیم یافت نشد." });

            if (model.NewLogoFile != null)
            {
                DeleteFile(teamToUpdate.ImagePath);
                teamToUpdate.ImagePath = await UploadFileAsync(model.NewLogoFile, "teams");
            }

            teamToUpdate.Name = model.Name;
            teamToUpdate.Stadium = model.Stadium;

            // Update the many-to-many relationship
            // 1. Clear the existing league entries
            teamToUpdate.LeagueEntries.Clear();
            // 2. Add the new ones from the form
            if (model.LeagueIds != null && model.LeagueIds.Any())
            {
                foreach (var leagueId in model.LeagueIds)
                {
                    teamToUpdate.LeagueEntries.Add(new LeagueEntry { LeagueId = leagueId });
                }
            }

            // We don't call Update() because the entity is already being tracked.
            // EF Core will automatically detect the changes to the LeagueEntries collection.
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
            if (file == null || file.Length == 0) return null;
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", subfolder);
            Directory.CreateDirectory(uploadsFolder);
            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return $"/images/{subfolder}/{uniqueFileName}";
        }

        private void DeleteFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;
            var fullPath = Path.Combine(_environment.WebRootPath, relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}