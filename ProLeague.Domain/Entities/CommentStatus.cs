// File: Domain/Entities/CommentStatus.cs
namespace ProLeague.Domain.Entities
{
    public enum CommentStatus
    {
        Pending,  // در انتظار تایید
        Approved, // تایید شده
        Rejected  // رد شده
    }
}