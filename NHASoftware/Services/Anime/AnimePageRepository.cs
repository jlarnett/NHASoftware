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
    private List<int>? _cachedIds;
    public async Task<AnimePage?> GetRandomEntityAsync()
    {
        if (_context.AnimePages == null)
            return null;

        if (_cachedIds == null || _cachedIds.Count == 0)
            _cachedIds = await _context.AnimePages.Select(g => g.Id).ToListAsync();

        if (_cachedIds.Count == 0)
            return null;

        int randomId = _cachedIds[Random.Shared.Next(_cachedIds.Count)];
        return await _context.AnimePages.FirstOrDefaultAsync(g => g.Id == randomId);
    }
}
