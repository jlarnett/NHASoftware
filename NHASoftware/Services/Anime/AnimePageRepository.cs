using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Anime;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Services.Anime;
public class AnimePageRepository : GenericRepository<AnimePage>, IAnimePageRepository
{
    public AnimePageRepository(ApplicationDbContext context) : base(context)
    {

    }

    public async Task<AnimePage?> GetRandomEntityAsync()
    {
        if (_context.AnimePages != null)
        {
            var maxId = await _context.AnimePages.MaxAsync(x => x.Id);
            var minId = await _context.AnimePages.MinAsync(x => x.Id);

            var random = new Random();
            int randomId = random.Next(minId, maxId + 1);

            // Retrieve the first entity with an ID greater than or equal to the random ID
            var randomEntity = _context.AnimePages
                .Where(x => x.Id >= randomId)
                .OrderBy(x => x.Id)
                .FirstOrDefault();

            // If no entity found with an ID >= randomId, try picking from the lower range
            randomEntity ??= _context.AnimePages
                    .Where(x => x.Id <= randomId)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault();

            return randomEntity;
        }

        return null;
    }
}
