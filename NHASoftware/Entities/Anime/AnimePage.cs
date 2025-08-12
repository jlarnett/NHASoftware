using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NHA.Website.Software.Entities.Anime;
public class AnimePage
{
    public int Id { get; set; }

    [DisplayName("Anime Name")]
    [MaxLength(2500)]
    public string AnimeName { get; set; } = string.Empty;
    
    [DisplayName("Anime Summary")]
    public string AnimeSummary { get; set; } = string.Empty;
    
    [DisplayName("Anime External Image Url")] 
    public string? AnimeImageUrl { get; set; } = string.Empty;
    
    [DisplayName("Anime Jikan Score")] 
    public double? AnimeJikanScore { get; set; } = 0;
    
    [DisplayName("Anime Airing Status")] 
    [MaxLength(200)]
    public string? AnimeStatus { get; set; } = string.Empty;
    
    [DisplayName("Anime Genres")] 
    [MaxLength(200)]
    public string? AnimeGenres { get; set; } = string.Empty;

    public int UpVotes { get; set; }
    public int DownVotes { get; set; }

    public bool Featured { get; set; } = false;
}
