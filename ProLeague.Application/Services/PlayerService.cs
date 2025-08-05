// ProLeague.Application/Services/PlayerService.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.Player;
using ProLeague.Domain.Entities;
using System.IO;

namespace ProLeague.Application.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;

        public PlayerService(IUnitOfWork unitOfWork, IWebHostEnvironment environment)
        {
            _unitOfWork = unitOfWork;
            _environment = environment;
        }
        public async Task<IEnumerable<Player>> GetAllPlayersWithTeamAsync()
        {
            return await _unitOfWork.Players.GetAllPlayersWithTeamAsync();
        }
        public async Task<Player?> GetPlayerWithTeamDetailsAsync(int id)
        {
            return await _unitOfWork.Players.GetPlayerWithTeamDetailsAsync(id);
        }
        public async Task<IEnumerable<Player>> GetAllPlayersAsync()
        {
            return await _unitOfWork.Players.GetAllAsync();
        }

        public async Task<Player?> GetPlayerByIdAsync(int id)
        {
            return await _unitOfWork.Players.GetByIdAsync(id);
        }

        public async Task<EditPlayerViewModel?> GetPlayerForEditAsync(int id)
        {
            var player = await _unitOfWork.Players.GetByIdAsync(id);
            if (player == null) return null;
            return new EditPlayerViewModel
            {
                Id = player.Id,
                Name = player.Name,
                Position = player.Position,
                TeamId = player.TeamId,
                Goals = player.Goals,
                Assists = player.Assists,
                ExistingPhotoPath = player.ImagePath
            };
        }

        public async Task<Result> CreatePlayerAsync(CreatePlayerViewModel model)
        {
            var photoPath = await UploadFileAsync(model.PhotoFile, "players");
            var player = new Player
            {
                Name = model.Name,
                Position = model.Position,
                TeamId = model.TeamId,
                ImagePath = photoPath,
                Goals = 0,
                Assists = 0
            };
            await _unitOfWork.Players.AddAsync(player);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        public async Task<Result> UpdatePlayerAsync(EditPlayerViewModel model)
        {
            var player = await _unitOfWork.Players.GetByIdAsync(model.Id);
            if (player == null) return Result.Failure(new[] { "بازیکن یافت نشد." });

            if (model.NewPhotoFile != null)
            {
                DeleteFile(player.ImagePath);
                player.ImagePath = await UploadFileAsync(model.NewPhotoFile, "players");
            }

            player.Name = model.Name;
            player.Position = model.Position;
            player.TeamId = model.TeamId;
            player.Goals = model.Goals;
            player.Assists = model.Assists;

            _unitOfWork.Players.Update(player);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        public async Task<Result> DeletePlayerAsync(int id)
        {
            var player = await _unitOfWork.Players.GetByIdAsync(id);
            if (player == null) return Result.Failure(new[] { "بازیکن یافت نشد." });

            DeleteFile(player.ImagePath);
            _unitOfWork.Players.Delete(player);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        private async Task<string?> UploadFileAsync(IFormFile? file, string subfolder) { /* ... */ return null; }
        private void DeleteFile(string? relativePath) { /* ... */ }
    }
}