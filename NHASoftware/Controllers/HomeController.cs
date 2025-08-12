using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using AutoMapper;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using NHA.Website.Software.Services.CookieMonster;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Services.Social.PostBuilderService;
using NHA.Website.Software.Views.ViewModels;
using NHA.Website.Software.ConsumableEntities.DTOs;
using Microsoft.AspNetCore.Authorization;
using NHA.Website.Software.Entities.ChatSystem;
using NHA.Website.Software.Services.ProfilePicture;
using NHA.Website.Software.Views.Shared.Social.ViewModels;
using NHA.Website.Software.Entities.Social_Entities;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NHA.Website.Software.Services.Anime;
using NHA.Website.Software.Services.Game;
using NHA.Website.Software.Services.Sponsors;
using NHA.Website.Software.Views.Shared.ChatSystem.ViewModels;

namespace NHA.Website.Software.Controllers;
public class HomeController : Controller
{
    private ILogger<HomeController> _logger;
    private readonly ICookieMonster _cookieMonster;
    private readonly IPostBuilder _postBuilder;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProfilePictureFileScrubber _scrubber;
    private readonly IMapper _mapper;


    public HomeController(ILogger<HomeController> logger,
        UserManager<ApplicationUser> userManager,
        ICookieMonster cookieMonster,
        IMapper mapper, IUnitOfWork unitOfWork, IPostBuilder postBuilder, IProfilePictureFileScrubber scrubber)
    {
        /*************************************************************************************
         *  Dependency injection services
         *************************************************************************************/

        _logger = logger;
        _cookieMonster = cookieMonster;
        _postBuilder = postBuilder;
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _scrubber = scrubber;
        _mapper = mapper;
    }

    public IActionResult Index()
    {
        CreateProfilePictureHangfireJob();
        CreateAnimeLoadHangfireJob();
        CreateGameLoadHangfireJob();
        CreateFeaturedAnimeSelectorJob();
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

        if (requestDTO.RecipientUserId.IsNullOrEmpty() || requestDTO.RecipientUserId.IsNullOrEmpty())
        {
            return BadRequest();
        }

        var friend = await _userManager.FindByIdAsync(requestDTO.RecipientUserId);
        var chatMessages =
            await _unitOfWork.ChatMessageRepository.GetChatMessagesAsync(requestDTO.SenderUserId,
                requestDTO.RecipientUserId);

        var chatMessageDTOs = chatMessages.Select(_mapper.Map<ChatMessage, ChatMessageDTO>).ToList();

        ChatUIViewModel vm = new ChatUIViewModel(friend, chatMessageDTOs);
        var updateResult = await UpdateChatMessageToSeen(chatMessages);

        if (!updateResult)
            _logger.LogTrace($"Failed to update chat message seen value in DB {DateTime.UtcNow}");

        return PartialView("ChatSystem/_ChatUI", vm);

    }

    private async Task<bool> UpdateChatMessageToSeen(List<ChatMessage> chatMessages)
    {
        foreach (var chat in chatMessages)
        {
            if (!chat.MessageViewedByRecipient && _userManager.GetUserId(User)!.Equals(chat.RecipientUserId))
            {
                chat.MessageViewedByRecipient = true;
                _unitOfWork.ChatMessageRepository.Update(chat);
            }
        }
        var result = await _unitOfWork.CompleteAsync();
        return result > 0;
    }

    /// <summary>
    /// Makes sure the recurring profile picture scrubber hangfire job is scheduled weekly
    /// </summary>
    private void CreateProfilePictureHangfireJob() =>
        RecurringJob.AddOrUpdate<IProfilePictureFileScrubber>("ProfilePictureScrubber", x=> x.RemoveOldProfilePicturesFromFolder(), Cron.Hourly);

    /// <summary>
    /// Make sure the recurring anime load hangfire job is scheduled weekly
    /// </summary>
    private void CreateAnimeLoadHangfireJob() =>
        RecurringJob.AddOrUpdate<IAnimeLeecher>("AnimeLeecher", x=> x.LoadExternalAnime(), Cron.Weekly);

    /// <summary>
    /// Make sure the recurring game load hangfire job is scheduled weekly
    /// </summary>
    private void CreateGameLoadHangfireJob() =>
        RecurringJob.AddOrUpdate<IGameLeecher>("GameLeecher", x=> x.LoadExternalGameInformation(), Cron.Weekly);

    /// <summary>
    /// Make sure the recurring Featured anime job runs every 2 days at midnight
    /// </summary>
    private void CreateFeaturedAnimeSelectorJob() =>
        RecurringJob.AddOrUpdate<IAdMaximizerService>("FeaturedAnimeSelector", x => x.PickFeaturedAnime(), Cron.Hourly);

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
