using NHA.Website.Software.Entities.Anime;

namespace NHA.Website.Software.Views.Anime.Vms
{
    public class IndexViewModel
    {
        public IEnumerable<AnimePage> AnimeList { get; set; } = [];
    }
}
