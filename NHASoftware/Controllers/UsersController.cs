using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHA.Website.Software.Services.FriendSystem;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHA.Website.Software.Views.ViewModels.FriendVMs;
using NHASoftware.Entities.Identity;
using NHASoftware.Views.ViewModels.SocialVMs;

namespace NHA.Website.Software.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFriendSystem _friendSystem;

        public UsersController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IMapper mapper, IFriendSystem friendSystem)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _friendSystem = friendSystem;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfiles(string? userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var posts = _unitOfWork.PostRepository.Find(p => p.UserId!.Equals(user.Id));

            var profileVM = new ProfileVM()
            {
                User = user,
            };

            return user != null ? View("Profiles", profileVM) : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> FriendsView(string? userId)
        {
            if (userId == null) return NotFound();

            var user = await _userManager.FindByIdAsync(userId);
            var friendList = await _friendSystem.GetUsersFriendListAsync(userId);
            var friendListVM = new FriendListVM(user, friendList);

            return View("Friends", friendListVM);
        }

        [HttpGet]
        public async Task<IActionResult> MutualFriendsView(string? userIdOne, string? userIdTwo)
        {
            if (userIdOne == null || userIdTwo == null) return NotFound();
            var modelProfileUser = await _userManager.FindByIdAsync(userIdOne);
            var mutualFriendList = await _friendSystem.GetMutualFriendsAsync(userIdOne, userIdTwo);
            var mutualFriendListVM = new MutualFriendListVM(modelProfileUser, mutualFriendList);

            return View("MutualFriends", mutualFriendListVM);
        }
    }
}
