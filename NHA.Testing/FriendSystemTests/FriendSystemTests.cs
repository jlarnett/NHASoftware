using System.Linq.Expressions;
using AutoMapper;
using NHA.Website.Software.ConsumableEntities.DTOs;
using NHA.Website.Software.Entities.FriendSystem;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Services.FriendSystem;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NSubstitute;

namespace NHA.Unit.Tests.FriendSystemTests
{
    [TestClass]
    public class FriendSystemTests
    {
        private IUnitOfWork _unitOfWork = null!;
        private IFriendRequestRepository _friendRequestRepository = null!;
        private IFriendRepository _friendRepository = null!;
        private IMapper _mapper = null!;
        private FriendSystem _sut = null!;

        [TestInitialize]
        public void Setup()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _friendRequestRepository = Substitute.For<IFriendRequestRepository>();
            _friendRepository = Substitute.For<IFriendRepository>();
            _mapper = Substitute.For<IMapper>();

            _unitOfWork.FriendRequestRepository.Returns(_friendRequestRepository);
            _unitOfWork.FriendRepository.Returns(_friendRepository);

            _friendRequestRepository.AddAsync(Arg.Any<FriendRequest>()).Returns(Task.CompletedTask);
            _friendRepository.AddAsync(Arg.Any<Friends>()).Returns(Task.CompletedTask);

            _sut = new FriendSystem(_unitOfWork, _mapper);
        }

        [TestMethod]
        public async Task FriendRequestAlreadySent()
        {
            var pendingRequest = new FriendRequest
            {
                SenderUserId = "sender",
                RecipientUserId = "recipient",
                Status = FriendRequestStatuses.Inprogress
            };

            _friendRequestRepository
                .FindAsync(Arg.Any<Expression<Func<FriendRequest, bool>>>())
                .Returns(Task.FromResult<IEnumerable<FriendRequest>>(new[] { pendingRequest }));

            var result = await _sut.IsFriendRequestSentAsync("sender", "recipient");

            Assert.IsTrue(result);
        }

        [TestMethod]
        public async Task SendFriendRequestAsync_ReturnsFalse_WhenRequestAlreadyExists()
        {
            var dto = new FriendRequestDTO
            {
                SenderUserId = "sender",
                RecipientUserId = "recipient"
            };

            _friendRequestRepository
                .FindAsync(Arg.Any<Expression<Func<FriendRequest, bool>>>())
                .Returns(Task.FromResult<IEnumerable<FriendRequest>>([
                    new FriendRequest
                    {
                        SenderUserId = "sender",
                        RecipientUserId = "recipient",
                        Status = FriendRequestStatuses.Inprogress
                    }
                ]));

            var result = await _sut.SendFriendRequestAsync(dto);

            Assert.IsFalse(result);
            await _friendRequestRepository.DidNotReceive().AddAsync(Arg.Any<FriendRequest>());
        }

        [TestMethod]
        public async Task SendFriendRequestAsync_AddsRequestAndReturnsTrue_WhenRequestIsValid()
        {
            var dto = new FriendRequestDTO
            {
                SenderUserId = "sender",
                RecipientUserId = "recipient",
                Status = FriendRequestStatuses.Inprogress
            };

            var mappedRequest = new FriendRequest
            {
                SenderUserId = "sender",
                RecipientUserId = "recipient",
                Status = FriendRequestStatuses.Inprogress
            };

            _friendRequestRepository
                .FindAsync(Arg.Any<Expression<Func<FriendRequest, bool>>>())
                .Returns(Task.FromResult<IEnumerable<FriendRequest>>(Array.Empty<FriendRequest>()));

            _friendRepository
                .FindAsync(Arg.Any<Expression<Func<Friends, bool>>>())
                .Returns(Task.FromResult<IEnumerable<Friends>>(Array.Empty<Friends>()));

            _mapper.Map<FriendRequestDTO, FriendRequest>(dto).Returns(mappedRequest);
            _unitOfWork.CompleteAsync().Returns(1);

            var result = await _sut.SendFriendRequestAsync(dto);

            Assert.IsTrue(result);
            await _friendRequestRepository.Received(1).AddAsync(mappedRequest);
        }

        [TestMethod]
        public async Task AcceptFriendRequestAsync_ReturnsFalse_WhenIdIsNull()
        {
            var result = await _sut.AcceptFriendRequestAsync(null);

            Assert.IsFalse(result);
        }

        [TestMethod]
        public async Task AcceptFriendRequestAsync_CreatesFriendshipAndReturnsTrue_WhenRequestExists()
        {
            var friendRequest = new FriendRequest
            {
                Id = 10,
                SenderUserId = "sender",
                RecipientUserId = "recipient",
                Status = FriendRequestStatuses.Inprogress
            };

            _friendRequestRepository
                .GetByIdAsync(10)
                .Returns(Task.FromResult<FriendRequest?>(friendRequest));

            _friendRepository
                .FindAsync(Arg.Any<Expression<Func<Friends, bool>>>())
                .Returns(Task.FromResult<IEnumerable<Friends>>(Array.Empty<Friends>()));

            _unitOfWork.CompleteAsync().Returns(1);

            var result = await _sut.AcceptFriendRequestAsync(10);

            Assert.IsTrue(result);
            Assert.AreEqual(FriendRequestStatuses.Accepted, friendRequest.Status);

            await _friendRepository.Received(1).AddAsync(
                Arg.Is<Friends>(f =>
                    f.FriendOneId == "sender" &&
                    f.FriendTwoId == "recipient"));
        }

        [TestMethod]
        public async Task AcceptFriendRequestAsync_ReturnsFalse_WhenUsersAreAlreadyFriends()
        {
            var friendRequest = new FriendRequest
            {
                Id = 11,
                SenderUserId = "sender",
                RecipientUserId = "recipient",
                Status = FriendRequestStatuses.Inprogress
            };

            _friendRequestRepository
                .GetByIdAsync(11)
                .Returns(Task.FromResult<FriendRequest?>(friendRequest));

            _friendRepository
                .FindAsync(Arg.Any<Expression<Func<Friends, bool>>>())
                .Returns(Task.FromResult<IEnumerable<Friends>>(new[]
                {
                    new Friends
                    {
                        FriendOneId = "sender",
                        FriendTwoId = "recipient"
                    }
                }));

            var result = await _sut.AcceptFriendRequestAsync(11);

            Assert.IsFalse(result);
            await _friendRepository.DidNotReceive().AddAsync(Arg.Any<Friends>());
        }

        [TestMethod]
        public async Task DeclineFriendRequestAsync_UpdatesStatusAndReturnsTrue()
        {
            var friendRequest = new FriendRequest
            {
                Id = 12,
                SenderUserId = "sender",
                RecipientUserId = "recipient",
                Status = FriendRequestStatuses.Inprogress
            };

            _friendRequestRepository
                .GetByIdAsync(12)
                .Returns(Task.FromResult<FriendRequest?>(friendRequest));

            _unitOfWork.CompleteAsync().Returns(1);

            var result = await _sut.DeclineFriendRequestAsync(12);

            Assert.IsTrue(result);
            Assert.AreEqual(FriendRequestStatuses.Declined, friendRequest.Status);
        }

        [TestMethod]
        public async Task DeleteFriendRequestAsync_RemovesRequestAndReturnsTrue()
        {
            var friendRequest = new FriendRequest
            {
                Id = 13,
                SenderUserId = "sender",
                RecipientUserId = "recipient"
            };

            _friendRequestRepository
                .GetByIdAsync(13)
                .Returns(Task.FromResult<FriendRequest?>(friendRequest));

            _unitOfWork.CompleteAsync().Returns(1);

            var result = await _sut.DeleteFriendRequestAsync(13);

            Assert.IsTrue(result);
            _friendRequestRepository.Received(1).Remove(friendRequest);
        }

        [TestMethod]
        public async Task CancelFriendRequestAsync_UpdatesStatusAndReturnsTrue()
        {
            var dto = new FriendRequestDTO
            {
                SenderUserId = "sender",
                RecipientUserId = "recipient"
            };

            var friendRequest = new FriendRequest
            {
                Id = 14,
                SenderUserId = "sender",
                RecipientUserId = "recipient",
                Status = FriendRequestStatuses.Inprogress
            };

            _friendRequestRepository
                .FindAsync(Arg.Any<Expression<Func<FriendRequest, bool>>>())
                .Returns(Task.FromResult<IEnumerable<FriendRequest>>(new[] { friendRequest }));

            _unitOfWork.CompleteAsync().Returns(1);

            var result = await _sut.CancelFriendRequestAsync(dto);

            Assert.IsTrue(result);
            Assert.AreEqual(FriendRequestStatuses.Canceled, friendRequest.Status);
        }

        [TestMethod]
        public async Task GetFriendCountAsync_ReturnsRepositoryCount()
        {
            _friendRepository
                .CountAsync(Arg.Any<Expression<Func<Friends, bool>>>())
                .Returns(3);

            var result = await _sut.GetFriendCountAsync("user-1");

            Assert.AreEqual(3, result);
        }

        [TestMethod]
        public async Task GetUsersFriendListAsync_ReturnsOnlyTheUsersFriends()
        {
            var userTwo = new ApplicationUser { Id = "user-2", UserName = "user2" };
            var userThree = new ApplicationUser { Id = "user-3", UserName = "user3" };

            var friendRecords = new List<Friends>
            {
                new Friends
                {
                    FriendOneId = "user-1",
                    FriendTwoId = "user-2",
                    FriendTwo = userTwo
                },
                new Friends
                {
                    FriendOneId = "user-3",
                    FriendTwoId = "user-1",
                    FriendOne = userThree
                }
            };

            _friendRepository
                .GetUsersFriendListAsync("user-1")
                .Returns(Task.FromResult(friendRecords));

            var result = await _sut.GetUsersFriendListAsync("user-1");

            Assert.AreEqual(2, result.Count);
            Assert.IsTrue(result.Any(u => u.Id == "user-2"));
            Assert.IsTrue(result.Any(u => u.Id == "user-3"));
            Assert.IsFalse(result.Any(u => u.Id == "user-1"));
        }

        [TestMethod]
        public async Task GetMutualFriendsAsync_ReturnsOnlySharedFriends()
        {
            var mutualFriend = new ApplicationUser { Id = "mutual", UserName = "mutualUser" };
            var onlyUserOneHas = new ApplicationUser { Id = "only-1", UserName = "only1" };
            var onlyUserTwoHas = new ApplicationUser { Id = "only-2", UserName = "only2" };

            _friendRepository
                .GetUsersFriendListAsync("user-1")
                .Returns(Task.FromResult(new List<Friends>
                {
                    new Friends
                    {
                        FriendOneId = "user-1",
                        FriendTwoId = "mutual",
                        FriendTwo = mutualFriend
                    },
                    new Friends
                    {
                        FriendOneId = "user-1",
                        FriendTwoId = "only-1",
                        FriendTwo = onlyUserOneHas
                    }
                }));

            _friendRepository
                .GetUsersFriendListAsync("user-2")
                .Returns(Task.FromResult(new List<Friends>
                {
                    new Friends
                    {
                        FriendOneId = "user-2",
                        FriendTwoId = "mutual",
                        FriendTwo = mutualFriend
                    },
                    new Friends
                    {
                        FriendOneId = "user-2",
                        FriendTwoId = "only-2",
                        FriendTwo = onlyUserTwoHas
                    }
                }));

            var result = await _sut.GetMutualFriendsAsync("user-1", "user-2");

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("mutual", result[0].Id);
        }
    }
}
