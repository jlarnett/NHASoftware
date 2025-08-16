using NHA.Website.Software.Entities.Game;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;

namespace NHA.Website.Software.Services.Game;
public interface IGamePageRepository : IGenericRepository<GamePage>
{
    Task<GamePage?> GetRandomEntityAsync();
}
