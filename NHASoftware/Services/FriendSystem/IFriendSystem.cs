using NHA.Website.Software.ConsumableEntities.DTOs;
using NHA.Website.Software.Entities.Identity;
namespace NHA.Website.Software.Services.FriendSystem;
public interface IFriendSystem
{
    Task<bool> AcceptFriendRequestAsync(int? id);
    Task<bool> DeclineFriendRequestAsync(int? id);
    Task<bool> DeleteFriendRequestAsync(int? id);
    Task<bool> IsFriendRequestSentAsync(string senderId, string recipientId);
    Task<List<FriendRequestDTO>> GetPendingFriendRequestsAsync(string userId);
    Task<bool> IsFriendsAsync(string senderId, string recipientId);
    Task<bool> SendFriendRequestAsync(FriendRequestDTO friendRequest);
    Task<int> GetFriendCountAsync(string userId);
    Task<bool> RemoveFriendsAsync(FriendRequestDTO friendRequestDto);
    Task<bool> CancelFriendRequestAsync(FriendRequestDTO friendRequestDto);
    Task<List<ApplicationUser>> GetMutualFriendsAsync(string userIdOne, string userIdTwo);
    public Task<List<ApplicationUser>> GetUsersFriendListAsync(string userId);
}
