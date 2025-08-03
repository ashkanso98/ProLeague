// File: Domain/Entities/NewsComment.cs

using System.ComponentModel.DataAnnotations;

namespace ProLeague.Domain.Entities;

public class NewsComment
{
    public int Id { get; set; }
    [Required]
    public string Content { get; set; } = null!;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    // فیلد جدید برای وضعیت نظر
    public CommentStatus Status { get; set; } = CommentStatus.Pending;

    public string UserId { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
    public int NewsId { get; set; }
    public News News { get; set; } = null!;
}