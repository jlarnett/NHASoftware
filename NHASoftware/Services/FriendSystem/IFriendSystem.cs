using NHASoftware.ConsumableEntities.DTOs;

namespace NHASoftware.Services.FriendSystem
{
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
    }
}