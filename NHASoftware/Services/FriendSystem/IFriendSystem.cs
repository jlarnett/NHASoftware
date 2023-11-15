using NHA.Website.Software.ConsumableEntities.DTOs;
using NHA.Website.Software.Entities.Identity;
namespace NHA.Website.Software.Services.FriendSystem;
public interface IFriendSystem
{
    Task<bool> AcceptFriendRequestAsync(int? id);
    Task<bool> DeclineFriendRequestAsync(int? id);
    Task<bool> DeleteFriendRequestAsync(int? id);
    bool FriendRequestSent(string senderId, string recipientId);
    IEnumerable<FriendRequestDTO> GetPendingFriendRequests(string userId);
    bool IsFriends(string senderId, string recipientId);
    Task<bool> SendFriendRequestAsync(FriendRequestDTO friendRequest);
    int GetFriendCount(string userId);
    Task<bool> RemoveFriendsAsync(FriendRequestDTO friendRequestDto);
    Task<bool> CancelFriendRequestAsync(FriendRequestDTO friendRequestDto);
    Task<IEnumerable<ApplicationUser>> GetMutualFriendsAsync(string userIdOne, string userIdTwo);
    public Task<IEnumerable<ApplicationUser>> GetUsersFriendListAsync(string userId);
}
