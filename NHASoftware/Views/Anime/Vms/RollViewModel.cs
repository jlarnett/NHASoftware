using NHA.Website.Software.Entities.Anime;

namespace NHA.Website.Software.Views.Anime.Vms
{
    public class RollViewModel
    {
        public int Page { get; set; } = 1;
        public int TotalPages { get; set; }
        public List<AnimePage> RollAnime { get; set; } = [];
    }
}
