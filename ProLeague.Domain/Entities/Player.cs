using System.ComponentModel.DataAnnotations;
using ProLeague.Domain.Entities;

public enum Position
{
    Goalkeeper, Defender, Midfielder, Forward
}
public class Player
{
    public int Id { get; set; }
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    public Position Position { get; set; }
    public int Goals { get; set; }
    public int Assists { get; set; }
    public string? ImagePath { get; set; } // Path to the player photo
    public int TeamId { get; set; }
    public Team Team { get; set; } = null!; // Navigation property
    public ICollection<News> News { get; set; } = new List<News>();
}