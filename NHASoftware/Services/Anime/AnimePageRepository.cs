using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHASoftware.DBContext;
using NHASoftware.Entities.Anime;

namespace NHA.Website.Software.Services.Anime
{
    public class AnimePageRepository : GenericRepository<AnimePage>, IAnimePageRepository
    {
        public AnimePageRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
