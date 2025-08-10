// ProLeague.Application/Services/CommentService.cs
using ProLeague.Application.Interfaces;
using ProLeague.Domain.Entities;

namespace ProLeague.Application.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CommentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<NewsComment>> GetCommentsByStatusAsync(CommentStatus status)
        {
            return await _unitOfWork.Comments.GetCommentsByStatusAsync(status);
        }
        public async Task<NewsComment?> GetCommentByIdAsync(int id)
        {
            // تغییر به متدی که شامل Include است
            return await _unitOfWork.Comments.GetCommentDetailsAsync(id);
        }
        public async Task<Result> ApproveCommentAsync(int id)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(id);
            if (comment == null) return Result.Failure(new[] { "نظر یافت نشد." });

            comment.Status = CommentStatus.Approved;
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        public async Task<Result> RejectCommentAsync(int id)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(id);
            if (comment == null) return Result.Failure(new[] { "نظر یافت نشد." });

            comment.Status = CommentStatus.Rejected;
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        public async Task<Result> DeleteCommentAsync(int id)
        {
            var comment = await _unitOfWork.Comments.GetByIdAsync(id);
            if (comment == null) return Result.Failure(new[] { "نظر یافت نشد." });

            _unitOfWork.Comments.Delete(comment);
            await _unitOfWork.CompleteAsync();
            return Result.Success();
        }

        public async Task<Result> SubmitCommentAsync(int newsId, string content, string userId)
        {
            try
            {
                var newsExists = await _unitOfWork.News.GetByIdAsync(newsId);
                if (newsExists == null)
                {
                    return Result.Failure(new[] { "خبر مورد نظر یافت نشد." });
                }

                var comment = new NewsComment
                {
                    NewsId = newsId,
                    Content = content,
                    UserId = userId,
                    CreatedDate = DateTime.UtcNow,
                    Status = CommentStatus.Pending // کامنت به صورت پیش‌فرض در صف تایید قرار می‌گیرد
                };

                await _unitOfWork.Comments.AddAsync(comment);
                await _unitOfWork.CompleteAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                // Log the exception (ex)
                return Result.Failure(new[] { "خطایی در هنگام ثبت نظر رخ داد." });
            }
        }
    }
}