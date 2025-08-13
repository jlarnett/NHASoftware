using NHA.Website.Software.Entities.Anime;
using NHA.Website.Software.Entities.Game;

namespace NHA.Website.Software.Controllers.WebAPIs.Search
{
    public class SearchResponse
    {
        public IEnumerable<AnimePage> AnimePages { get; set; } = [];
        public IEnumerable<GamePage> GamePages { get; set; } = [];

        public SearchResponse(IEnumerable<AnimePage> anime, IEnumerable<GamePage> games)
        {
            AnimePages = anime;
            GamePages = games;
        }
    }
}
