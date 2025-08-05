using System.ComponentModel;
namespace NHA.Website.Software.Entities.Anime;
public class AnimePage
{
    public int Id { get; set; }

    [DisplayName("Anime Name")] public string AnimeName { get; set; } = string.Empty;
    [DisplayName("Anime Summary")] public string AnimeSummary { get; set; } = string.Empty;
    [DisplayName("Anime External Image Url")] public string? AnimeImageUrl { get; set; } = string.Empty;

    public int UpVotes { get; set; }
    public int DownVotes { get; set; }
}
