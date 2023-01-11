using NHASoftware.Entities.FriendSystem;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHASoftware.Services.FriendSystem
{
    public interface IFriendRequestRepository : IGenericRepository<FriendRequest>
    {
        IEnumerable<FriendRequest> GetUsersPendingFriendRequest(string userId);
    }
}
