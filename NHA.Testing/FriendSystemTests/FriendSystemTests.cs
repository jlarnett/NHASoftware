using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHASoftware.Services.FriendSystem;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHA.Testing.FriendSystemTests
{
    [TestClass]
    public class FriendSystemTests
    {
        private readonly FriendSystem _friendSystem;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();

        public FriendSystemTests()
        {
            _friendSystem = new FriendSystem(_unitOfWorkMock.Object, _mapperMock.Object);
        }


        [TestMethod]
        public void FriendRequestAlreadySent()
        {
            string userIdOne = Guid.NewGuid().ToString();
            string userIdTwo = Guid.NewGuid().ToString();

        }
    }
}
