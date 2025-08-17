using NHA.Website.Software.Entities.Anime;
using NHA.Website.Software.Entities.Game;
using NHA.Website.Software.Entities.Identity;

namespace NHA.Website.Software.Controllers.WebAPIs.Search
{
    public class SearchResponse
    {
        public IEnumerable<AnimePage> AnimePages { get; set; }
        public IEnumerable<GamePage> GamePages { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; } = [];

        public SearchResponse(IEnumerable<AnimePage> anime, IEnumerable<GamePage> games, IEnumerable<ApplicationUser> users)
        {
            AnimePages = anime;
            GamePages = games;
            Users = users;
        }
    }
}
