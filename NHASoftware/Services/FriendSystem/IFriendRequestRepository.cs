using NHA.Website.Software.Entities.FriendSystem;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Services.FriendSystem;
public interface IFriendRequestRepository : IGenericRepository<FriendRequest>
{
    public Task<List<FriendRequest>> GetUsersPendingFriendRequestAsync(string userId);
}
