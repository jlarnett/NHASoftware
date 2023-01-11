using AutoMapper;
using NHASoftware.ConsumableEntities.DTOs;
using NHASoftware.Entities.FriendSystem;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHASoftware.Services.FriendSystem
{
    public class FriendSystem : IFriendSystem
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FriendSystem(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Checks whether two users have a pending friend request.
        /// </summary>
        /// <param name="senderId">Application User Id 1</param>
        /// <param name="recipientId">Application User Id 2</param>
        /// <returns></returns>
        public bool FriendRequestSent(string senderId, string recipientId)
        {
            return _unitOfWork.FriendRequestRepository
                .Find(f => f.RecipientUserId == recipientId && f.SenderUserId == senderId && f.Status == FriendRequestStatuses.Inprogress).Any();
        }

        /// <summary>
        /// Checks if two application users already have friend table entry.
        /// </summary>
        /// <param name="senderId">Application User Id 1</param>
        /// <param name="recipientId">Application User Id 2</param>
        /// <returns>True if users are friends already.</returns>
        public bool IsFriends(string senderId, string recipientId)
        {
            var isFriends = _unitOfWork.FriendRepository
                .Find(f => f.FriendOneId == senderId && f.FriendTwoId == recipientId || f.FriendOneId == recipientId && f.FriendTwoId == senderId)
                .Any();

            return isFriends;
        }

        /// <summary>
        /// Sends/Creates friend request between application users. 
        /// </summary>
        /// <param name="friendRequestDto">New Friend Request Dto</param>
        /// <returns></returns>
        public async Task<bool> SendFriendRequestAsync(FriendRequestDTO friendRequestDto)
        {
            if (!FriendRequestSent(friendRequestDto.SenderUserId, friendRequestDto.RecipientUserId) && !IsFriends(friendRequestDto.SenderUserId, friendRequestDto.RecipientUserId))
            {
                var friendRequest = _mapper.Map<FriendRequestDTO, FriendRequest>(friendRequestDto);
                _unitOfWork.FriendRequestRepository.Add(friendRequest);
                var dbChanges = await _unitOfWork.CompleteAsync();
                return dbChanges > 0;
            }

            return false;
        }

        /// <summary>
        /// Accepts friend request. Updates Friend Request Status & Creates new friend table entry.
        /// </summary>
        /// <param name="id">Friend Request Id to accept</param>
        /// <returns></returns>
        public async Task<bool> AcceptFriendRequestAsync(int? id)
        {
            if (id == null)
            {
                return false;
            }

            var friendRequest = await _unitOfWork.FriendRequestRepository.GetByIdAsync(id);

            if (friendRequest != null)
            {

                if (!IsFriends(friendRequest.SenderUserId, friendRequest.RecipientUserId))
                {
                    friendRequest.Status = FriendRequestStatuses.Accepted;

                    _unitOfWork.FriendRepository.Add(new Friends()
                    {
                        FriendOneId = friendRequest.SenderUserId,
                        FriendTwoId = friendRequest.RecipientUserId
                    });

                    var rowsChanged = await _unitOfWork.CompleteAsync();
                    return rowsChanged > 0;
                }
                
                return false;

            }

            return false;
        }

        /// <summary>
        /// Returns IEnumerable with all waiting friend request for supplied userId.
        /// </summary>
        /// <param name="userId">recipient userId</param>
        /// <returns>IEnumerable of FriendRequestDtos</returns>
        public IEnumerable<FriendRequestDTO> GetPendingFriendRequests(string userId)
        {
            var requests = _unitOfWork.FriendRequestRepository.GetUsersPendingFriendRequest(userId);
            return _mapper.Map<IEnumerable<FriendRequest>, IEnumerable<FriendRequestDTO>>(requests);
        }

        /// <summary>
        /// Deletes Friend Request from DB.
        /// </summary>
        /// <param name="id">Friend Request Id to delete</param>
        /// <returns>returns true if friend request was deleted</returns>
        public async Task<bool> DeleteFriendRequestAsync(int? id)
        {
            var friendRequest = await _unitOfWork.FriendRequestRepository.GetByIdAsync(id);

            if (friendRequest != null)
            {
                _unitOfWork.FriendRequestRepository.Remove(friendRequest);
                var changes = await _unitOfWork.CompleteAsync();
                return changes > 0;
            }

            return false;
        }

        /// <summary>
        /// Declines the supplied friend request Id. 
        /// </summary>
        /// <param name="id">Friend Request Id to decline</param>
        /// <returns>Boolean whether friend request was declined or failed to be declined</returns>
        public async Task<bool> DeclineFriendRequestAsync(int? id)
        {
            var friendRequest = await _unitOfWork.FriendRequestRepository.GetByIdAsync(id);

            if (friendRequest != null)
            {
                friendRequest.Status = FriendRequestStatuses.Declined;
                var changes = await _unitOfWork.CompleteAsync();

                return changes > 0;
            }

            return false;
        }
    }
}
