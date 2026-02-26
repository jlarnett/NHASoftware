using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace NHA.Website.Software.Entities.Anime;
public class AnimeEpisode
{
    public int Id { get; set; }

    [DisplayName("Episode Number")]
    public int EpisodeNumber { get; set; }

    [DisplayName("Title")]
    [MaxLength(100)]
    public string EpisodeName { get; set; } = string.Empty;

    [DisplayName("Summary")]
    [MaxLength(1000)]
    public string EpisodeSummary { get; set; } = string.Empty;

    [DisplayName("Contains Filler")]
    public bool EpisodeContainsFiller { get; set; }
    public int UpVotes { get; set; }
    public int DownVotes { get; set; }
    public AnimePage? AnimePage { get; set; }
    public int AnimePageId { get; set; }
}
