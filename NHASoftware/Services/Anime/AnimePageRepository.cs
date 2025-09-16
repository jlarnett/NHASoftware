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

    public async Task<IEnumerable<AnimePage>> GetResultPageAsync(int pageNumber = 1, int pageSize = 25)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        return await _context.Set<AnimePage>()
            .OrderByDescending(t => t.AnimeJikanScore)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }
}
