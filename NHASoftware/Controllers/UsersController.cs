using AutoMapper;
using Hangfire.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHA.Website.Software.Entities.Forums;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Services.FriendSystem;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHA.Website.Software.Services.SessionHistory;
using NHA.Website.Software.Views.ViewModels.FriendVMs;
using NHASoftware.Views.ViewModels.SocialVMs;
namespace NHA.Website.Software.Controllers;
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IFriendSystem _friendSystem;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IActiveSessionTracker _sessionTracker;
    private readonly ILogger<UsersController> _logger;


    public UsersController(UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IFriendSystem friendSystem,
        SignInManager<ApplicationUser> signInManager,
        IActiveSessionTracker sessionTracker,
        ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _friendSystem = friendSystem;
        _sessionTracker = sessionTracker;
        _signInManager = signInManager;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfiles(string? userId)
    {
        var user = await _userManager.FindByIdAsync(userId!);
        var posts = _unitOfWork.PostRepository.Find(p => p.UserId!.Equals(user!.Id));

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
        var friendListVM = new FriendListVM(user!, friendList);

        return View("Friends", friendListVM);
    }

    [HttpGet]
    public async Task<IActionResult> MutualFriendsView(string? userIdOne, string? userIdTwo)
    {
        if (userIdOne == null || userIdTwo == null) return NotFound();
        var modelProfileUser = await _userManager.FindByIdAsync(userIdOne);
        var mutualFriendList = await _friendSystem.GetMutualFriendsAsync(userIdOne, userIdTwo);
        var mutualFriendListVM = new MutualFriendListVM(modelProfileUser!, mutualFriendList);
        return View("MutualFriends", mutualFriendListVM);
    }

    public async Task<IActionResult> Logout(string? returnUrl = null)
    {
        var currentPrinciple = _signInManager.Context.User;
        var user = await _signInManager.UserManager.GetUserAsync(currentPrinciple);

        if (user != null)
        {
            await _sessionTracker.CreateLogoutEvent(user.Email!);
        }

        await _signInManager.SignOutAsync();
        _logger.LogInformation("User logged out.");


        if (returnUrl != null)
        {
            return LocalRedirect(returnUrl);
        }
        else
        {
            // This needs to be a redirect so that the browser performs a new
            // request and the identity for the user gets updated.
            return RedirectToPage("/");
        }
    }

}
