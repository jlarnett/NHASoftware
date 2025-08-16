using NHA.Website.Software.Entities.Anime;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Services.Anime;
public interface IAnimePageRepository : IGenericRepository<AnimePage>
{
    Task<AnimePage?> GetRandomEntityAsync();
}
