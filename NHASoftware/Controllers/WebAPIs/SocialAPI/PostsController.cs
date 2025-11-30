using AutoMapper;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.FeatureManagement.Mvc;
using NHA.Helpers.ImageDataSourceTranslator;
using NHA.Website.Software.Caching;
using NHA.Website.Software.ConsumableEntities.DTOs;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Entities.Social_Entities;
using NHA.Website.Software.Services.CacheLoadingManager;
using NHA.Website.Software.Services.CookieMonster;
using NHA.Website.Software.Services.FileExtensionValidator;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHA.Website.Software.Services.Social.PostBuilderService;
using System.Text.RegularExpressions;

namespace NHA.Website.Software.Controllers.WebAPIs.SocialAPI;
[Route("api/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger _logger;
    private readonly IFileExtensionValidator _fileExtensionValidator;
    private readonly IImageDataSourceTranslator _imageDataSourceTranslator;
    private readonly IMemoryCache _memoryCache;
    private readonly ICacheLoadingManager _cacheLoadingManager;
    private readonly IPostBuilder _postBuilder;
    private readonly ICookieMonster _cookieMonster;

    public PostsController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager,
        ILogger<PostDTO> logger, IFileExtensionValidator validator,
        IImageDataSourceTranslator imageTranslator, IMemoryCache memoryCache,
        ICacheLoadingManager cacheLoadingManager, IPostBuilder postBuilder,
        ICookieMonster cookieMonster)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _logger = logger;
        _fileExtensionValidator = validator;
        _imageDataSourceTranslator = imageTranslator;
        _memoryCache = memoryCache;
        _cacheLoadingManager = cacheLoadingManager;
        _postBuilder = postBuilder;
        _cookieMonster = cookieMonster;
    }

    [HttpGet]
    public async Task<ActionResult<List<Post>>> GetPosts()
    {
        var posts = (await _unitOfWork.PostRepository.GetAllPostsWithIncludesAsync()).Where(p => p.ParentPostId == null);
        return Ok(posts);
    }

    [HttpGet("GetPostImages/{id}")]
    public async Task<ActionResult<List<string>>> GetPostImages(int? id)
    {
        if (id == null)
            return NotFound();

        var images = await _unitOfWork.PostImageRepository.GetPostImagesAsync(id);
        List<string> imageDataSources = new List<string>();

        foreach (var image in images)
        {
            var imageDataSource =
                _imageDataSourceTranslator.GetDataSourceTranslation(image.FileExtensionType, image.ImageBytes!);
            imageDataSources.Add(imageDataSource);
        }

        return Ok(imageDataSources);
    }

    // PUT: api/Posts/5
    [HttpPut("{id}")]
    public async Task<IActionResult> PutPost(int? id, PostDTO postDto)
    {
        var post = _mapper.Map<PostDTO, Post>(postDto);

        if (id != post.Id)
        {
            return BadRequest();
        }

        _unitOfWork.PostRepository.Update(post);

        try
        {
            await _unitOfWork.CompleteAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PostExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    /// <summary>
    /// POST: api/Posts
    /// API Endpoint for creating new social media post.
    /// </summary>
    /// <param name="postdto"></param>
    /// <returns>Returns IActionResult with new post. </returns>
    [HttpPost("BasicPost")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> CreateBasicPost([FromForm] PostDTO postdto)
    {
        string textOnly = Regex.Replace(postdto.Summary, "<.*?>", string.Empty);

        if (textOnly.Length < 10)
        {
            return BadRequest(new
                { success = false, message = "Post summary must be at least 10 characters long" });
        }

        var post = AssignServerSidePostParameters(postdto);
        await _unitOfWork.PostRepository.AddAsync(post);
        var result = await _unitOfWork.CompleteAsync();

        if (result > 0)
        {
            var newlyCreatedPost = await _postBuilder.LocateNewlyCreatedPost(post);
            _cacheLoadingManager.IncrementCacheChangeCounter(CachingKeys.Posts);

            _logger.Log(LogLevel.Information, "Post API successfully added new post to DB {post}", newlyCreatedPost);
            return Created($"/api/posts/{newlyCreatedPost.Id}", new
            {
                success = true,
                post = newlyCreatedPost,
                message = "Post successfully submitted to DB."
            });
        } 

        _logger.Log(LogLevel.Debug, "system was unable to add postDto to DB.");
        return BadRequest(new { success = false , message = "POST API returned bad request. Post was not saved to DB."});
    }

    /// <summary>
    /// POST: api/Posts/CustomizedPosts
    /// API Endpoint for creating new custom social media post. This is the endpoint used for creating post with images attached.
    /// </summary>
    /// <param name="postdto"></param>
    /// <returns>Returns IActionResult with new post. </returns>
    [HttpPost("CustomizedPost")]
    [ValidateAntiForgeryToken]
    [Authorize]
    [FeatureGate("CustomizedPostsEnabled")]
    public async Task<IActionResult> CreateCustomizedPost([FromForm] PostDTO postdto)
    {
        string textOnly = Regex.Replace(postdto.Summary, "<.*?>", string.Empty);

        if (textOnly.Length < 10)
        {
            return BadRequest(new
                { success = false, message = "Post summary must be at least 10 characters long" });
        }

        var imageFilesIncluded = postdto.ImageFiles != null && postdto.ImageFiles.Count > 0;

        if (imageFilesIncluded)
        {
            foreach (var imageFile in postdto.ImageFiles!)
            {
                if (!_fileExtensionValidator.CheckValidImageExtensions(imageFile.FileName))
                    return BadRequest(new
                    { success = false, message = "Unable To Submit Custom Post - Image file extension not supported." });
            }
        }

        var post = AssignServerSidePostParameters(postdto);
        await _unitOfWork.PostRepository.AddAsync(post);
        var result = await _unitOfWork.CompleteAsync();

        if (result > 0)
        {
            var newlyCreatedPost = await _postBuilder.LocateNewlyCreatedPost(post);

            if (imageFilesIncluded)
            {
                var imageSaveResult = await SavePostImagesToDatabase(newlyCreatedPost.Id, postdto.ImageFiles!);
                if (!imageSaveResult)
                    return BadRequest(new { success = false, message = "Error Saving Images To DB" });
            }

            var newPost = await _postBuilder.LocateNewlyCreatedPost(post);
            _cacheLoadingManager.IncrementCacheChangeCounter(CachingKeys.Posts);

            _logger.Log(LogLevel.Information, "Post API successfully added new post to DB {post}", newPost);

            return Created($"/api/posts/{newPost.Id}", new
            {
                success = true,
                post = newPost,
                message = "Post successfully submitted to DB."
            });
        }

        _logger.Log(LogLevel.Debug, "system was unable to add postDto to DB.");
        return BadRequest(new { success = false, message = "Unable to submit post. Post was not saved to DB - Bad Request"});
    }

    /// <summary>
    /// Used to set the isDeletedFlag on post object. Flag is being used to avoid hassles with EF self referencing table. 
    /// </summary>
    /// <param name="id">Id of the post to delete</param>
    /// <returns>Returns jsonresult with success value. </returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePost(int? id)
    {
        var post = await _unitOfWork.PostRepository.GetByIdAsync(id);
        _logger.Log(LogLevel.Information, $"attempted to execute - {nameof(DeletePost)}, parameters - {HttpContext.Request.Body}");

        if (post == null)
        {
            _logger.Log(LogLevel.Debug, $"Failed to execute {nameof(DeletePost)} post id not found");
            return NotFound();
        }

        post.IsDeletedFlag = true;
        _unitOfWork.PostRepository.Update(post);
        var result = await _unitOfWork.CompleteAsync();

        if (result > 0)
        {
            _logger.Log(LogLevel.Information, "Post was deleted from DB successfully.");
            _cacheLoadingManager.IncrementCacheChangeCounter(CachingKeys.Posts);
            return Ok(new { success = true });
        }
        else
        {
            _logger.Log(LogLevel.Debug, "error happened trying to delete post from DB");
            return BadRequest(new { success = false });
        }
    }

    /// <summary>
    /// Used to set the isHiddenFromUserProfile on post object. Flag is being used to avoid hassles with EF self referencing table. 
    /// </summary>
    /// <param name="id">Id of the post to hide</param>
    /// <returns>IActionResult with success indicator whether post was successfully hidden or not. </returns>
    [HttpDelete("Hide/{id}")]
    public async Task<IActionResult> HidePost(int? id)
    {
        var post = await _unitOfWork.PostRepository.GetByIdAsync(id);

        if (post == null)
        {
            return NotFound();
        }

        var sessionId = _cookieMonster.TryRetrieveCookie(CookieKeys.Session);

        post.IsHiddenFromUserProfile = true;
        _unitOfWork.PostRepository.Update(post);
        var result = await _unitOfWork.CompleteAsync();

        return result > 0 ? Ok(new { success = true }) : BadRequest(new { success = false });
    }

    /// <summary>
    /// Reactivates social media post. Changes the isDeletedFlag of object in db. 
    /// </summary>
    /// <param name="id">id of the post the developer wants reactivated. </param>
    /// <returns>IActionResult with success indicator whether post was successfully reactivated or not. </returns>
    [HttpDelete("Reactivate/{id}")]
    public async Task<IActionResult> ReactivatePost(int? id)
    {
        var post = await _unitOfWork.PostRepository.GetByIdAsync(id);

        if (post == null)
        {
            return NotFound();
        }

        post.IsDeletedFlag = false;
        _unitOfWork.PostRepository.Update(post);
        var result = await _unitOfWork.CompleteAsync();

        return result > 0 ? Ok(new { success = true }) : BadRequest(new { success = false });
    }

    private bool PostExists(int? id)
    {
        var post = _unitOfWork.PostRepository.Find(p => p.Id.Equals(id));
        return post.Any();
    }

    private Post AssignServerSidePostParameters(PostDTO postdto)
    {
        var post = _mapper.Map<PostDTO, Post>(postdto);
        var creationDate = DateTime.UtcNow;
        post.CreationDate = creationDate;
        post.UserId = _userManager.GetUserId(User);

        return post;
    }

    private async Task<bool> SavePostImagesToDatabase(int? postId, List<IFormFile> imageFiles)
    {
        List<PostImage> images = new List<PostImage>();

        foreach (var imageFile in imageFiles)
        {
            using var memoryStream = new MemoryStream();
            await imageFile.CopyToAsync(memoryStream);

            // Upload the file if less than 5 MB
            if (memoryStream.Length < 5242880)
            {
                PostImage postImage = new()
                {
                    ImageBytes = memoryStream.ToArray(),
                    PostId = postId,
                    FileExtensionType = Path.GetExtension(imageFile.FileName)
                };

                images.Add(postImage);
            }

        }

        await _unitOfWork.PostImageRepository.AddRange(images);
        var savePostImageResult = await _unitOfWork.CompleteAsync();

        if (savePostImageResult > 0)
        {
            return true;
        }

        return false;
    }

}
