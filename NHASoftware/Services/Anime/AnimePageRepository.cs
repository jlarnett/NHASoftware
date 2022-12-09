using NHASoftware.DBContext;
using NHASoftware.Entities.Anime;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHASoftware.Services.Anime
{
    public class AnimePageRepository : GenericRepository<AnimePage>, IAnimePageRepository
    {
        public AnimePageRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
