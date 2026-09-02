using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Session;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Services.SessionHistory;

public class SessionHistoryRepository : GenericRepository<SessionHistoryEvent>, ISessionHistoryRepository
{
    public SessionHistoryRepository(ApplicationDbContext context) : base(context)
    {

    }

    /// <summary>
    /// Accesses the EF context & gets all social media post. DOES NOT INCLUDE POST WITH ISDELETEDFLAG set to true
    /// </summary>
    /// <returns></returns>
    public async Task<List<SessionHistoryEvent>> GetSortedSessionActivityForUserAsync(string userId) =>
        await _context.Set<SessionHistoryEvent>().Where(s => s.userId.Equals(userId)).OrderByDescending(s => s.Time).ToListAsync();

    public async Task<DateTime?> GetLastSessionActivityForUserAsync(string userId) =>
        await _context.Set<SessionHistoryEvent>()
            .Where(s => s.userId == userId)
            .MaxAsync(s => (DateTime?)s.Time);

    public async Task<Dictionary<string, DateTime?>> GetLastSessionActivityForUsersAsync(IEnumerable<string> userIds)
    {
        var distinctUserIds = userIds
            .Where(userId => !string.IsNullOrWhiteSpace(userId))
            .Distinct()
            .ToArray();

        if (distinctUserIds.Length == 0)
        {
            return new Dictionary<string, DateTime?>();
        }

        var lastActiveTimes = await _context.Set<SessionHistoryEvent>()
            .Where(s => distinctUserIds.Contains(s.userId))
            .GroupBy(s => s.userId)
            .Select(g => new
            {
                UserId = g.Key,
                LastActiveTime = (DateTime?)g.Max(x => x.Time)
            })
            .ToDictionaryAsync(x => x.UserId, x => x.LastActiveTime);

        foreach (var userId in distinctUserIds)
        {
            lastActiveTimes.TryAdd(userId, null);
        }

        return lastActiveTimes;
    }
}
