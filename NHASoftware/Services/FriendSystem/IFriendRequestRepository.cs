using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHASoftware.Entities.FriendSystem;

namespace NHA.Website.Software.Services.FriendSystem
{
    public interface IFriendRequestRepository : IGenericRepository<FriendRequest>
    {
        IEnumerable<FriendRequest> GetUsersPendingFriendRequest(string userId);
    }
}
