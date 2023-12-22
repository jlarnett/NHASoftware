using NHA.Website.Software.Entities.Session;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;

namespace NHA.Website.Software.Services.SessionHistory;
public interface ISessionHistoryRepository : IGenericRepository<SessionHistoryEvent>
{
    Task<List<SessionHistoryEvent>> GetSortedSessionActivityForUserAsync(string userId);
}
