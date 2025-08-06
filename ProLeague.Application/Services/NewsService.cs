// ProLeague.Application/Services/NewsService.cs
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ProLeague.Application.Interfaces;
using ProLeague.Application.ViewModels.News;
using ProLeague.Domain.Entities;
using System.IO;

namespace ProLeague.Application.Services
{
    public class NewsService : INewsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _environment;

        public NewsService(IUnitOfWork unitOfWork, IWebHostEnvironment environment)
        {
            _unitOfWork = unitOfWork;
            _environment = environment;
        }

        public async Task<IEnumerable<News>> GetAllNewsAsync()
        {
            return await _unitOfWork.News.GetAllAsync();
        }

        public async Task<News?> GetNewsDetailsAsync(int id)
        {
            return await _unitOfWork.News.GetNewsDetailsAsync(id);
        }

        public async Task<EditNewsViewModel?> GetNewsForEditAsync(int id)
        {
            var news = await _unitOfWork.News.GetNewsDetailsAsync(id);
            if (news == null) return null;

            return new EditNewsViewModel
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content,
                IsFeatured = news.IsFeatured,
                ExistingMainImagePath = news.MainImagePath,
                ExistingGalleryImages = news.Images.ToList(),
                RelatedLeagueIds = news.RelatedLeagues.Select(l => l.Id).ToList(),
                RelatedTeamIds = news.RelatedTeams.Select(t => t.Id).ToList(),
                RelatedPlayerIds = news.RelatedPlayers.Select(p => p.Id).ToList(),
            };
        }

        public async Task<Result> CreateNewsAsync(CreateNewsViewModel model)
        {
            var news = new News
            {
                Title = model.Title,
                Content = model.Content,
                IsFeatured = model.IsFeatured,
                PublishedDate = DateTime.UtcNow,
                MainImagePath = await UploadFileAsync(model.MainImageFile, "news/main")
            };

            if (model.GalleryFiles != null)
            {
                foreach (var file in model.GalleryFiles)
                {
                    news.Images.Add(new NewsImage { ImagePath = (await UploadFileAsync(file, "news/gallery"))! });
                }
            }

            await UpdateRelatedEntities(news, model.RelatedLeagueIds, model.RelatedTeamIds, model.RelatedPlayerIds);

            await _unitOfWork.News.AddAsync(news);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }
        public async Task<Result> UpdateNewsAsync(EditNewsViewModel model)
        {
            var newsToUpdate = await _unitOfWork.News.GetNewsDetailsAsync(model.Id);
            if (newsToUpdate == null) return Result.Failure(new[] { "خبر یافت نشد." });

            if (model.NewMainImageFile != null && model.NewMainImageFile.Length > 0)
            {
                DeleteFile(newsToUpdate.MainImagePath);
                newsToUpdate.MainImagePath = await UploadFileAsync(model.NewMainImageFile, "news/main");
            }

            if (model.NewGalleryFiles != null)
            {
                foreach (var file in model.NewGalleryFiles)
                {
                    if (file != null && file.Length > 0)
                    {
                        var imagePath = await UploadFileAsync(file, "news/gallery");
                        if (imagePath != null)
                        {
                            newsToUpdate.Images.Add(new NewsImage { ImagePath = imagePath });
                        }
                    }
                }
            }

            newsToUpdate.Title = model.Title;
            newsToUpdate.Content = model.Content;
            newsToUpdate.IsFeatured = model.IsFeatured;

            await UpdateRelatedEntities(newsToUpdate, model.RelatedLeagueIds, model.RelatedTeamIds, model.RelatedPlayerIds);

            // _unitOfWork.News.Update(newsToUpdate); // <<--- این خط باید حذف شود

            await _unitOfWork.CompleteAsync(); // ردیاب به صورت خودکار تمام تغییرات را ذخیره می‌کند
            return Result.Success();
        }
        //***********
        //public async Task<Result> UpdateNewsAsync(EditNewsViewModel model)
        //{
        //    var newsToUpdate = await _unitOfWork.News.GetNewsDetailsAsync(model.Id);
        //    if (newsToUpdate == null) return Result.Failure(new[] { "خبر یافت نشد." });

        //    // ۱. بررسی فایل اصلی جدید با دقت بیشتر
        //    if (model.NewMainImageFile != null && model.NewMainImageFile.Length > 0)
        //    {
        //        DeleteFile(newsToUpdate.MainImagePath);
        //        newsToUpdate.MainImagePath = await UploadFileAsync(model.NewMainImageFile, "news/main");
        //    }

        //    // ۲. بررسی فایل‌های گالری جدید با دقت بیشتر
        //    if (model.NewGalleryFiles != null)
        //    {
        //        foreach (var file in model.NewGalleryFiles)
        //        {
        //            // این شرط بسیار مهم است و از خطای SQL جلوگیری می‌کند
        //            if (file != null && file.Length > 0)
        //            {
        //                var imagePath = await UploadFileAsync(file, "news/gallery");
        //                if (imagePath != null)
        //                {
        //                    newsToUpdate.Images.Add(new NewsImage { ImagePath = imagePath });
        //                }
        //            }
        //        }
        //    }

        //    newsToUpdate.Title = model.Title;
        //    newsToUpdate.Content = model.Content;
        //    newsToUpdate.IsFeatured = model.IsFeatured;

        //    await UpdateRelatedEntities(newsToUpdate, model.RelatedLeagueIds, model.RelatedTeamIds, model.RelatedPlayerIds);

        //    _unitOfWork.News.Update(newsToUpdate);
        //    await _unitOfWork.CompleteAsync();
        //    return Result.Success();
        //}


        public async Task<Result> DeleteNewsAsync(int id)
        {
            var news = await _unitOfWork.News.GetNewsDetailsAsync(id);
            if (news == null) return Result.Failure(new[] { "خبر یافت نشد." });

            DeleteFile(news.MainImagePath);
            foreach (var image in news.Images)
            {
                DeleteFile(image.ImagePath);
            }

            _unitOfWork.News.Delete(news);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        public async Task<Result> DeleteGalleryImageAsync(int imageId)
        {
            var image = await _unitOfWork.News.GetNewsImageByIdAsync(imageId);
            if (image == null) return Result.Failure(new[] { "تصویر یافت نشد." });

            DeleteFile(image.ImagePath);
            _unitOfWork.News.DeleteNewsImage(image);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        private async Task UpdateRelatedEntities(News news, List<int>? leagueIds, List<int>? teamIds, List<int>? playerIds)
        {
            // ... پیاده سازی مشابه قبل برای بروزرسانی روابط ...
        }
        private async Task<string?> UploadFileAsync(IFormFile? file, string subfolder) { /* ... */ return null; }
        private void DeleteFile(string? relativePath) { /* ... */ }
    }
}