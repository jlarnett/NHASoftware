using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.FriendSystem;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Services.FriendSystem;
public class FriendRequestRepository : GenericRepository<FriendRequest>, IFriendRequestRepository
{
    public FriendRequestRepository(ApplicationDbContext context) : base(context)
    {
    }
    /// <summary>
    /// Returns all friend request with INP status. Includes Both Application User details
    /// </summary>
    /// <param name="userId">Friend Request's Recipient Userid</param>
    /// <returns></returns>
    public IEnumerable<FriendRequest> GetUsersPendingFriendRequest(string userId)
    {
        return _context.FriendRequests!.Include(fq => fq.RecipientUser).Include(fq => fq.SenderUser).Where(fq => fq.RecipientUserId.Equals(userId) && fq.Status == FriendRequestStatuses.Inprogress).AsEnumerable();
    }
}
