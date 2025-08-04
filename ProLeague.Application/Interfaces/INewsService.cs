// ProLeague.Application/Interfaces/INewsService.cs
using ProLeague.Application.ViewModels.News;
using ProLeague.Domain.Entities;

namespace ProLeague.Application.Interfaces
{
    public interface INewsService
    {
        Task<IEnumerable<News>> GetAllNewsAsync();
        Task<News?> GetNewsDetailsAsync(int id);
        Task<EditNewsViewModel?> GetNewsForEditAsync(int id);
        Task<Result> CreateNewsAsync(CreateNewsViewModel model);
        Task<Result> UpdateNewsAsync(EditNewsViewModel model);
        Task<Result> DeleteNewsAsync(int id);
        Task<Result> DeleteGalleryImageAsync(int imageId);
    }
}