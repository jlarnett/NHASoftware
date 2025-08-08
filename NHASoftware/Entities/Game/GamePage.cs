using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NHA.Website.Software.Entities.Game;
public class GamePage
{
    public int Id { get; set; }

    [DisplayName("Game Name")]
    [MaxLength(2500)]
    public string Name { get; set; } = string.Empty;
    
    [DisplayName("Game Summary")]
    public string Summary { get; set; } = string.Empty;
    
    [DisplayName("Game External Image Url")] 
    public string? ImageUrl { get; set; } = string.Empty;
    
    [DisplayName("Game Score")] 
    public double? GameScore { get; set; } = 0;
    
    [DisplayName("Game Status")] 
    [MaxLength(200)]
    public string? Status { get; set; } = string.Empty;
    
    [DisplayName("Game Genres")] 
    [MaxLength(200)]
    public string? Genres { get; set; } = string.Empty;

    [DisplayName("Platform")]
    [MaxLength(200)]
    public string? Platforms { get; set; } = string.Empty;

    [DisplayName("Date Released")] 
    [MaxLength(50)]
    public string? Released { get; set; } = string.Empty;

    public int UpVotes { get; set; }
    public int DownVotes { get; set; }
}