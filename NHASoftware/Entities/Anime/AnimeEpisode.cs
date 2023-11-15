namespace NHA.Website.Software.Entities.Anime;
public class AnimeEpisode
{
    public int Id { get; set; }
    public int EpisodeNumber { get; set; }
    public string EpisodeName { get; set; } = string.Empty;
    public string EpisodeSummary { get; set; } = string.Empty;
    public bool EpisodeContainsFiller { get; set; }
    public int UpVotes { get; set; }
    public int DownVotes { get; set; }
    public AnimePage? AnimePage { get; set; }
    public int AnimePageId { get; set; }
}
