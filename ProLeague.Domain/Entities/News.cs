using System.ComponentModel.DataAnnotations;
using ProLeague.Domain.Entities;

public class News
{
    public int Id { get; set; }
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required] public string Content { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; } = DateTime.UtcNow;
    public bool IsFeatured { get; set; } // For highlighting important news on homepage
    public string? MainImagePath { get; set; } // Path to the main news image
    public ICollection<NewsImage> Images { get; set; } = new List<NewsImage>();
    public ICollection<NewsComment> Comments { get; set; } = new List<NewsComment>();
    public ICollection<League> RelatedLeagues { get; set; } = new List<League>();
    public ICollection<Team> RelatedTeams { get; set; } = new List<Team>();
    public ICollection<Player> RelatedPlayers { get; set; } = new List<Player>();
}