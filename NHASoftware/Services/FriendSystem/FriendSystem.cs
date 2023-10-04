﻿using AutoMapper;
using NHA.Website.Software.Entities.FriendSystem;
using NHASoftware.ConsumableEntities.DTOs;
using NHASoftware.Entities.FriendSystem;
using NHASoftware.Entities.Identity;
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

        /// <summary>
        /// Returns INT with the # of friends the supplied userId has.
        /// </summary>
        /// <param name="userId">Identity UserId you want to check friend count for</param>
        /// <returns>int # of friends for specified userId</returns>
        public int GetFriendCount(string userId)
        {
            return _unitOfWork.FriendRepository.Count(f => f.FriendOneId.Equals(userId) || f.FriendTwoId.Equals(userId));
        }

        /// <summary>
        /// Removes pair of friends from DB. 
        /// </summary>
        /// <param name="friendRequestDto">Friend Request Dto that contains sender & recipient ids. </param>
        /// <returns>Bool whether the friendship was able to be removed or not. </returns>
        public async Task<bool> RemoveFriendsAsync(FriendRequestDTO friendRequestDto)
        {
            var friendRecords = _unitOfWork.FriendRepository.Find(f =>
                (f.FriendOneId.Equals(friendRequestDto.RecipientUserId) &&
                 f.FriendTwoId.Equals(friendRequestDto.SenderUserId) ||
                 (f.FriendOneId.Equals(friendRequestDto.SenderUserId) &&
                  f.FriendTwoId.Equals(friendRequestDto.RecipientUserId))));

            var friendEntity = friendRecords.FirstOrDefault();

            if (friendEntity != null)
            {
                _unitOfWork.FriendRepository.Remove(friendEntity);
                var changes = await _unitOfWork.CompleteAsync();
                return changes > 0;
            }

            return false;
        }

        /// <summary>
        /// Changes friend request status to canceled in DB. 
        /// </summary>
        /// <param name="friendRequestDto">Friend Request Dto that contains sender & recipient ids. </param>
        /// <returns>Bool whether the friend request was canceled or not.  </returns>
        public async Task<bool> CancelFriendRequestAsync(FriendRequestDTO friendRequestDto)
        {
            var friendRequest = _unitOfWork.FriendRequestRepository.Find(fq =>
                fq.RecipientUserId.Equals(friendRequestDto.RecipientUserId) &&
                fq.SenderUserId.Equals(friendRequestDto.SenderUserId) &&
                fq.Status.Equals(FriendRequestStatuses.Inprogress)).FirstOrDefault();

            if (friendRequest != null)
            {
                friendRequest.Status = FriendRequestStatuses.Canceled;
                var changes = await _unitOfWork.CompleteAsync();
                return changes > 0;
            }

            return false;
        }

        /// <summary>
        /// Gets list of mutual friends for two application users. 
        /// </summary>
        /// <param name="userIdOne">AppUser 1 id</param>
        /// <param name="userIdTwo">AppUser 2 id</param>
        /// <returns>List of all mutual friends between the two specified users. </returns>
        public async Task<IEnumerable<ApplicationUser>> GetMutualFriendsAsync(string userIdOne, string userIdTwo)
        {
            var userFriendships = await _unitOfWork.FriendRepository.GetUsersFriendListAsync(userIdOne);
            var userTwoFriendships = await _unitOfWork.FriendRepository.GetUsersFriendListAsync(userIdTwo);

            var userFriends = ReturnListOfUsersFriends(userIdOne, userFriendships);
            var userTwoFriends = ReturnListOfUsersFriends(userIdTwo, userTwoFriendships);

            return CompareMutualFriends(userFriends, userTwoFriends);
        }

        /// <summary>
        /// Takes in list of friend records and strips the owning appUser to return list of only the user's friends. 
        /// </summary>
        /// <param name="userId">UserId to strip</param>
        /// <param name="friendList">list of friend records pulled from DB. </param>
        /// <returns>List of applicationUsers that only contains the user's friends but not the user. </returns>
        private List<ApplicationUser> ReturnListOfUsersFriends(string userId, IEnumerable<Friends> friendList)
        {
            List<ApplicationUser> friends = new List<ApplicationUser>();

            if (friendList.Any())
            {
                foreach (var friendship in friendList)
                {
                    if (friendship.FriendOneId.Equals(userId))
                    {
                        if (friendship.FriendTwo != null)
                        {
                            friends.Add(friendship.FriendTwo);
                        }
                    }
                    else
                    {
                        if (friendship.FriendOne != null)
                        {
                            friends.Add(friendship.FriendOne);
                        }
                    }
                }
            }

            return friends;
        }

        /// <summary>
        /// Compares two friend lists & returns a List of all matching mutual friends (Application Users)
        /// </summary>
        /// <param name="userOneFriendsList"></param>
        /// <param name="userTwoFriendsList"></param>
        /// <returns>Returns list of mutual friends. </returns>
        private List<ApplicationUser> CompareMutualFriends(List<ApplicationUser> userOneFriendsList,
            List<ApplicationUser> userTwoFriendsList)
        {
            HashSet<ApplicationUser> friends = new HashSet<ApplicationUser>();
            List<ApplicationUser> mutualFriends = new List<ApplicationUser>();

            foreach (var user in userOneFriendsList)
            {
                //Adds all of users 1s friends to hash set for mutual friend comparison.
                friends.Add(user);
            }

            foreach (var potentialMutualFriend in userTwoFriendsList)
            {
                if (friends.Contains(potentialMutualFriend))
                {
                    //If user is already a part of the hashset that means there is a duplicate entry and thus mutual friends.
                    mutualFriends.Add(potentialMutualFriend);
                }
            }

            return mutualFriends;
        }
    }
}
