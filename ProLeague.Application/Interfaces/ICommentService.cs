// ProLeague.Application/Interfaces/ICommentService.cs
using ProLeague.Domain.Entities;

namespace ProLeague.Application.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<NewsComment>> GetCommentsByStatusAsync(CommentStatus status);
        Task<NewsComment?> GetCommentByIdAsync(int id);
        Task<Result> ApproveCommentAsync(int id);
        Task<Result> RejectCommentAsync(int id);
        Task<Result> DeleteCommentAsync(int id);
        Task<Result> SubmitCommentAsync(int newsId, string content, string userId);
    }
}