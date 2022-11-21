namespace NHASoftware.Models.AnimeModels
{
    public class AnimeEpisode
    {
        public int Id { get; set; }
        public int EpisodeNumber { get; set; }
        public string EpisodeName { get; set; }
        public string EpisodeSummary { get; set; }
        public bool EpisodeContainsFiller { get; set; }
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
        public AnimePage? AnimePage { get; set; }
        public int AnimePageId { get; set; }
    }
}
