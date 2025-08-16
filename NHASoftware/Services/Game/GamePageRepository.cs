using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Game;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;

namespace NHA.Website.Software.Services.Game;
public class GamePageRepository : GenericRepository<GamePage>, IGamePageRepository
{
    public GamePageRepository(ApplicationDbContext context) : base(context)
    {

    }
    private List<int>? _cachedGameIds;

    public async Task<GamePage?> GetRandomEntityAsync()
    {
        if (_context.GamePages == null)
            return null;

        // Load or refresh cached IDs
        if (_cachedGameIds == null || _cachedGameIds.Count == 0)
            _cachedGameIds = await _context.GamePages.Select(g => g.Id).ToListAsync();

        if (_cachedGameIds.Count == 0)
            return null;

        // Pick a random ID from the cached list
        int randomId = _cachedGameIds[Random.Shared.Next(_cachedGameIds.Count)];

        // Fetch the actual GamePage entity
        return await _context.GamePages.FirstOrDefaultAsync(g => g.Id == randomId);
    }
}
