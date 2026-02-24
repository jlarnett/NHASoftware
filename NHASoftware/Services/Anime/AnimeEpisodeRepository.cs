using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Anime;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Services.Anime;
public class AnimeEpisodeRepository : GenericRepository<AnimeEpisode>, IAnimeEpisodeRepository
{
    public AnimeEpisodeRepository(ApplicationDbContext context) : base(context)
    {

    }
}
