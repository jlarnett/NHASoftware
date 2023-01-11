using Microsoft.EntityFrameworkCore;
using NHASoftware.DBContext;
using NHASoftware.Entities.FriendSystem;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHASoftware.Services.FriendSystem
{
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
            return _context.FriendRequests.Include(fq => fq.RecipientUser).Include(fq => fq.SenderUser).Where(fq => fq.RecipientUserId.Equals(userId) && fq.Status == FriendRequestStatuses.Inprogress).AsEnumerable();
        }
    }
}
