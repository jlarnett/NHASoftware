using NHA.Website.Software.Entities.FriendSystem;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Services.FriendSystem;
public interface IFriendRequestRepository : IGenericRepository<FriendRequest>
{
    IEnumerable<FriendRequest> GetUsersPendingFriendRequest(string userId);
}
