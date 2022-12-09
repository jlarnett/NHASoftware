using System.ComponentModel;

namespace NHASoftware.Entities.Anime
{
    public class AnimePage
    {
        public int Id { get; set; }

        [DisplayName("Anime Name")]
        public string AnimeName { get; set; }

        [DisplayName("Anime Summary")]
        public string AnimeSummary { get; set; }
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
    }
}
