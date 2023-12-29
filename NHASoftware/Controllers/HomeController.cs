using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NHA.Website.Software.Services.CookieMonster;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Services.Social.PostBuilderService;
using NHA.Website.Software.Views.ViewModels;
using NHA.Website.Software.ConsumableEntities.DTOs;
using Microsoft.AspNetCore.Authorization;
using NHA.Website.Software.Views.Shared.Social.ViewModels;

namespace NHA.Website.Software.Controllers;
public class HomeController : Controller
{
    private ILogger<HomeController> _logger;
    private readonly ICookieMonster _cookieMonster;
    private readonly IPostBuilder _postBuilder;
    private readonly UserManager<ApplicationUser> _userManager;


    public HomeController(ILogger<HomeController> logger,
        UserManager<ApplicationUser> userManager,
        ICookieMonster cookieMonster,
        IMapper mapper, IUnitOfWork unitOfWork, IPostBuilder postBuilder)
    {
        /*************************************************************************************
         *  Dependency injection services
         *************************************************************************************/

        _logger = logger;
        _cookieMonster = cookieMonster;
        _postBuilder = postBuilder;
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        AssignSessionGuidCookie();
        return View();
    }

    /// <summary>
    /// Home/ReturnSocialPosts
    /// Gets all parent post from post builder & returns _MultiPost partial view result. 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> ReturnSocialPosts()
    {
        var postDTOs = await _postBuilder.RetrieveParentPosts();
        return PartialView("Social/_MultiPost", new MultiPostVM(postDTOs));
    }

    /// <summary>
    /// GET : Home/ReturnCommentPosts
    /// Gets all comments for specified post Id & returns partial view containing them all
    /// </summary>
    /// <param name="id">postId you want to retrieve comments for</param>
    /// <returns>_MultiComment partial view result</returns>
    [HttpGet]
    public async Task<IActionResult> ReturnCommentPosts(int? id, Guid? uuid)
    {
        if (id == null)
        {
            return BadRequest(new {sucess = false});
        }

        var postDTOs = await _postBuilder.FindPostChildren(id);
        return PartialView("Social/_MultiComment", new MultiPostVM(postDTOs, uuid));
    }

    /// <summary>
    /// GET : /GetAllPostForUser/userId
    /// Gets all social posts for supplied userId.
    /// Converts the list of postDTOs retrieved from postbuilder and returns _MultiPost partial view.
    /// </summary>
    /// <param name="userId">Users Identity Id you want posts for</param>
    /// <returns>_MultiPost partial view result</returns>
    [HttpGet("GetAllPostForUser/{userId}")]
    public async Task<IActionResult> GetAllPostForUser(string userId)
    {
        var postDTOs  = await _postBuilder.GetAllPostForUser(userId);
        return PartialView("Social/_MultiPost", new MultiPostVM(postDTOs));
    }

    /// <summary>
    /// POST: /ReturnPostPartialView
    /// Takes inputted postDTO & returns a _Post partial view. Basically converts DTO -> partial view.
    /// </summary>
    /// <param name="postdto">Fully Populated Post DTO that you want to receive partial view for</param>
    /// <returns>_Post Partial View Result</returns>
    [HttpPost("ReturnPostPartialView")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public IActionResult ReturnPostPartialView(PostDTO postdto)
    {
        return PartialView("Social/_Post", postdto);
    }

    /// <summary>
    /// POST: /ReturnCommentPartialView
    /// Takes inputted postDTO & returns a _PostComment partial view. Basically converts DTO -> partial view.
    /// </summary>
    /// <param name="postdto">Fully Populated Post DTO that you want to receive _PostComment partial view for</param>
    /// <returns>_PostComment Partial View Result</returns>
    [HttpPost("ReturnCommentPartialView")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public IActionResult ReturnCommentPartialView(PostDTO postdto)
    {
        return PartialView("Social/_PostComment", postdto);
    }

    [HttpGet("ReturnChatPartialView")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> ReturnChatPartialView(FriendRequestDTO requestDTO)
    {
        var friend = await _userManager.FindByIdAsync(requestDTO.RecipientUserId);
        return PartialView("ChatSystem/_ChatUI", friend);
    }

    

    private void AssignSessionGuidCookie()
    {
        _logger.LogInformation($"Trying to assign cookies at {DateTime.UtcNow.ToLongTimeString()}");
        var sessionId = _cookieMonster.TryRetrieveCookie(CookieKeys.Session);

        if (sessionId.Equals(string.Empty))
        {
            var sessionGuid = Guid.NewGuid();
            _cookieMonster.CreateCookie(CookieKeys.Session, sessionGuid.ToString());
        }
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
