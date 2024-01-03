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
        await _context.SessionHistory!.Where(s => s.userId.Equals(userId)).OrderByDescending(s => s.Time).ToListAsync();
}
