using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Game;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;

namespace NHA.Website.Software.Services.Game;
public class GamePageRepository : GenericRepository<GamePage>, IGamePageRepository
{
    public GamePageRepository(ApplicationDbContext context) : base(context)
    {
    }
}
