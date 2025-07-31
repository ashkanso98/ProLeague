using System.ComponentModel.DataAnnotations;
using ProLeague.Domain.Entities;

public class NewsComment
{
    public int Id { get; set; }
    [Required]
    public string Content { get; set; } = null!;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public string UserId { get; set; } = null!; // Foreign key to AspNetUsers
    public ApplicationUser User { get; set; } = null!; // Navigation property
    public int NewsId { get; set; }
    public News News { get; set; } = null!; // Navigation property
}