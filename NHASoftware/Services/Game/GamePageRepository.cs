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

    public async Task<GamePage?> GetRandomEntityAsync()
    {
        if (_context.GamePages != null)
        {
            var maxId = await _context.GamePages.MaxAsync(x => x.Id);
            var minId = await _context.GamePages.MinAsync(x => x.Id);

            var random = new Random();
            int randomId = random.Next(minId, maxId + 1);

            // Retrieve the first entity with an ID greater than or equal to the random ID
            var randomEntity = _context.GamePages
                .Where(x => x.Id >= randomId)
                .OrderBy(x => x.Id)
                .FirstOrDefault();

            // If no entity found with an ID >= randomId, try picking from the lower range
            randomEntity ??= _context.GamePages
                .Where(x => x.Id <= randomId)
                .OrderByDescending(x => x.Id)
                .FirstOrDefault();

            return randomEntity;
        }

        return null;
    }
}
