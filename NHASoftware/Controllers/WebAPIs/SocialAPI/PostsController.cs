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
    private const long MaxImageFileSizeBytes = 5 * 1024 * 1024;
    private const long MaxVideoFileSizeBytes = 100 * 1024 * 1024;
    private const string PostMediaFolderName = "PostMedia";

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
    private readonly IWebHostEnvironment _webHostEnvironment;

    public PostsController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager,
        ILogger<PostDTO> logger, IFileExtensionValidator validator,
        IImageDataSourceTranslator imageTranslator, IMemoryCache memoryCache,
        ICacheLoadingManager cacheLoadingManager, IPostBuilder postBuilder,
        ICookieMonster cookieMonster, IWebHostEnvironment webHostEnvironment)
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
        _webHostEnvironment = webHostEnvironment;
    }

    [HttpGet]
    public async Task<ActionResult<List<Post>>> GetPosts()
    {
        var posts = (await _unitOfWork.PostRepository.GetAllPostsWithIncludesAsync()).Where(p => p.ParentPostId == null);
        return Ok(posts);
    }

    [HttpGet("GetPostImages/{id}")]
    public async Task<IActionResult> GetPostImages(int? id)
    {
        if (id == null)
            return NotFound();

        var images = await _unitOfWork.PostImageRepository.GetPostImagesAsync(id);
        var mediaDataSources = new List<object>();

        foreach (var image in images)
        {
            var isVideo = IsVideoFileExtension(image.FileExtensionType);
            var imageDataSource = !isVideo && image.ImageBytes != null
                ? _imageDataSourceTranslator.GetDataSourceTranslation(image.FileExtensionType, image.ImageBytes)
                : null;

            mediaDataSources.Add(new
            {
                id = image.Id,
                dataSource = imageDataSource,
                mediaUrl = isVideo ? GetVideoMediaUrl(image) : null,
                fileExtensionType = image.FileExtensionType,
                isVideo
            });
        }

        return Ok(mediaDataSources);
    }

    [HttpGet("GetPostMedia/{mediaId}")]
    public async Task<IActionResult> GetPostMedia(int? mediaId)
    {
        if (mediaId == null)
        {
            return NotFound();
        }

        var media = await _unitOfWork.PostImageRepository.GetPostMediaAsync(mediaId);

        if (media == null || string.IsNullOrWhiteSpace(media.FileExtensionType))
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(media.MediaPath))
        {
            var physicalPath = GetPhysicalMediaPath(media.MediaPath);

            if (System.IO.File.Exists(physicalPath))
            {
                return PhysicalFile(physicalPath, GetMediaContentType(media.FileExtensionType), enableRangeProcessing: true);
            }
        }

        if (media.ImageBytes == null)
        {
            return NotFound();
        }

        return File(media.ImageBytes, GetMediaContentType(media.FileExtensionType), enableRangeProcessing: true);
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
                { success = false, message = "Post must be more than 10 characters long." });
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
    /// API Endpoint for creating new custom social media post. This is the endpoint used for creating posts with media attached.
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
                { success = false, message = "Post must be more than 10 characters long." });
        }

        var mediaFilesIncluded = postdto.MediaFiles != null && postdto.MediaFiles.Count > 0;

        if (mediaFilesIncluded)
        {
            foreach (var mediaFile in postdto.MediaFiles!)
            {
                if (!TryValidatePostMediaFile(mediaFile, out var validationMessage))
                {
                    return BadRequest(new
                    { success = false, message = validationMessage });
                }
            }
        }

        var post = AssignServerSidePostParameters(postdto);
        await _unitOfWork.PostRepository.AddAsync(post);
        var result = await _unitOfWork.CompleteAsync();

        if (result > 0)
        {
            var newlyCreatedPost = await _postBuilder.LocateNewlyCreatedPost(post);

            if (mediaFilesIncluded)
            {
                var mediaSaveResult = await SavePostMediaToDatabase(newlyCreatedPost.Id, postdto.MediaFiles!);
                if (!mediaSaveResult)
                    return BadRequest(new { success = false, message = "Error saving post media to DB." });
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
    /// POST: api/Posts/Report
    /// API Endpoint for reporting social media posts
    /// </summary>
    /// <param name="postdto"></param>
    /// <returns>Returns IActionResult with new post. </returns>
    [HttpPost("Report")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> ReportPost([FromForm] ReportedPost reportedPostDto)
    {
        var userId = _userManager.GetUserId(User);

        if (userId == null)
            return BadRequest(new { success = false, message = "Failed to report post. UserId/Login is required to report" }); 
        if(reportedPostDto.PostId == null)
            return BadRequest(new { success = false, message = "Failed to report post. PostId is required" });

        var exists = await _unitOfWork.ReportedPostRepository.FindWithoutTrackingAsync(p =>
            p.PostId.Equals(reportedPostDto.PostId) && p.UserId!.Equals(userId));

        if (exists.Any())
        {
            return BadRequest(new { success = false, message = "Error: You can only report a post one time" });
        }

        await _unitOfWork.ReportedPostRepository.AddAsync(new ReportedPost()
        {
            UserId = userId,
            PostId = reportedPostDto.PostId,
            ReasonForReport = reportedPostDto.ReasonForReport,
            ExtraInformation = reportedPostDto.ExtraInformation,

        });

        var result = await _unitOfWork.CompleteAsync();

        if (result > 0)
        {
            return Ok(new { success = true, message = "Report successfully submitted" });
        }

        return BadRequest(new { success = false, message = "Failed to submit report for unknown reason" });

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
    /// Adds entry into HiddenPost table. Causes PostBuilder to remove posts from user session when loading
    /// </summary>
    /// <param name="id">Id of the post to hide</param>
    /// <returns>IActionResult with success indicator whether post was successfully hidden or not. </returns>
    [HttpPost("Hide/{id}")]
    public async Task<IActionResult> HidePost(int? id)
    {
        var post = await _unitOfWork.PostRepository.GetByIdAsync(id);

        if (post == null)
        {
            return NotFound();
        }

        var sessionId = _cookieMonster.TryRetrieveCookie(CookieKeys.Session);

        if(sessionId != null && id != null)
        {
            await _unitOfWork.HiddenPostRepository.AddAsync(new HiddenPost
            {
                SessionId = sessionId,
                PostId = id.Value
            });
            var result = await _unitOfWork.CompleteAsync();
            _cacheLoadingManager.IncrementCacheChangeCounter(CachingKeys.Posts);
            return result > 0 ? Ok(new { success = true, message = "Post was successfully hidden" }) : BadRequest(new { success = false, message = "Failed to add hidden post record to DB." });
        }
        else
        {
            return BadRequest(new { success = false, message = "Unable to hide post. Either sessionId or postId is null" });
        }
    }

    /// <summary>
    /// Removes entry from HiddenPost table. Causes PostBuilder to show hidden posts again for user session
    /// </summary>
    /// <param name="id">Id of the post to hide</param>
    /// <returns>IActionResult with success indicator whether post was successfully hidden or not. </returns>
    [HttpDelete("Unhide/{id}")]
    public async Task<IActionResult> UnhidePost(int? id)
    {
        var post = await _unitOfWork.PostRepository.GetByIdAsync(id);

        if (post == null)
        {
            return NotFound();
        }

        var sessionId = _cookieMonster.TryRetrieveCookie(CookieKeys.Session);
        var hiddenPostRecords = await _unitOfWork.HiddenPostRepository.FindAsync(hp => hp.SessionId.Equals(sessionId) && hp.PostId.Equals(id));

        if (hiddenPostRecords.Count() > 0)
        {
            foreach(var record in hiddenPostRecords)
            {
                _unitOfWork.HiddenPostRepository.Remove(record);
            }

            var result = await _unitOfWork.CompleteAsync();
            _cacheLoadingManager.IncrementCacheChangeCounter(CachingKeys.Posts);
            return result > 0 ? Ok(new { success = true, message = "Post was successfully unhidden" }) : BadRequest(new { success = false, message = "Failed to remove hidden post record from DB." });
        }
        else
        {
            return BadRequest(new { success = false, message = "Unable to unhide post. Either sessionId or postId is null" });
        }
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

    private static bool IsVideoFileExtension(string fileExtensionType)
    {
        return fileExtensionType.StartsWith(".mp4", StringComparison.OrdinalIgnoreCase)
            || fileExtensionType.StartsWith(".webm", StringComparison.OrdinalIgnoreCase)
            || fileExtensionType.StartsWith(".ogg", StringComparison.OrdinalIgnoreCase)
            || fileExtensionType.StartsWith(".mov", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetMediaContentType(string fileExtensionType)
    {
        return fileExtensionType.ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".jpg" => "image/jpeg",
            ".jpeg" => "image/jpeg",
            ".bmp" => "image/bmp",
            ".mp4" => "video/mp4",
            ".webm" => "video/webm",
            ".ogg" => "video/ogg",
            ".mov" => "video/quicktime",
            _ => "application/octet-stream"
        };
    }

    private Post AssignServerSidePostParameters(PostDTO postdto)
    {
        var post = _mapper.Map<PostDTO, Post>(postdto);
        var creationDate = DateTime.UtcNow;
        post.CreationDate = creationDate;
        post.UserId = _userManager.GetUserId(User);

        return post;
    }

    private bool TryValidatePostMediaFile(IFormFile mediaFile, out string validationMessage)
    {
        validationMessage = string.Empty;

        if (mediaFile.Length <= 0)
        {
            validationMessage = "Unable to submit custom post - one of the uploaded files is empty.";
            return false;
        }

        if (_fileExtensionValidator.CheckValidImageExtensions(mediaFile.FileName))
        {
            if (mediaFile.Length > MaxImageFileSizeBytes)
            {
                validationMessage = "Unable to submit custom post - image files must be 5 MB or smaller.";
                return false;
            }

            return true;
        }

        if (_fileExtensionValidator.CheckValidVideoExtensions(mediaFile.FileName))
        {
            if (mediaFile.Length > MaxVideoFileSizeBytes)
            {
                validationMessage = "Unable to submit custom post - video files must be 100 MB or smaller.";
                return false;
            }

            return true;
        }

        validationMessage = "Unable to submit custom post - file extension not supported. Please upload an image or supported video file.";
        return false;
    }

    private async Task<bool> SavePostMediaToDatabase(int? postId, List<IFormFile> mediaFiles)
    {
        List<PostImage> media = new List<PostImage>();
        List<string> createdFilePaths = new List<string>();

        try
        {
            foreach (var mediaFile in mediaFiles)
            {
                var fileExtension = Path.GetExtension(mediaFile.FileName).ToLowerInvariant();

                if (IsVideoFileExtension(fileExtension))
                {
                    var relativeMediaPath = await SaveVideoFileAsync(mediaFile);
                    createdFilePaths.Add(GetPhysicalMediaPath(relativeMediaPath));

                    media.Add(new PostImage
                    {
                        MediaPath = relativeMediaPath,
                        PostId = postId,
                        FileExtensionType = fileExtension
                    });

                    continue;
                }

                using var memoryStream = new MemoryStream();
                await mediaFile.CopyToAsync(memoryStream);

                PostImage postImage = new()
                {
                    ImageBytes = memoryStream.ToArray(),
                    PostId = postId,
                    FileExtensionType = fileExtension
                };

                media.Add(postImage);
            }

            await _unitOfWork.PostImageRepository.AddRange(media);
            var savePostImageResult = await _unitOfWork.CompleteAsync();

            if (savePostImageResult > 0)
            {
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save post media for post {postId}", postId);
        }

        foreach (var createdFilePath in createdFilePaths)
        {
            if (System.IO.File.Exists(createdFilePath))
            {
                System.IO.File.Delete(createdFilePath);
            }
        }

        return false;
    }

    private async Task<string> SaveVideoFileAsync(IFormFile mediaFile)
    {
        var fileExtension = Path.GetExtension(mediaFile.FileName).ToLowerInvariant();
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var relativeMediaPath = Path.Combine(PostMediaFolderName, uniqueFileName);
        var physicalMediaPath = GetPhysicalMediaPath(relativeMediaPath);
        var mediaDirectory = Path.GetDirectoryName(physicalMediaPath);

        if (!string.IsNullOrWhiteSpace(mediaDirectory))
        {
            Directory.CreateDirectory(mediaDirectory);
        }

        await using var fileStream = new FileStream(physicalMediaPath, FileMode.Create, FileAccess.Write, FileShare.None);
        await mediaFile.CopyToAsync(fileStream);

        return relativeMediaPath;
    }

    private string GetVideoMediaUrl(PostImage postImage)
    {
        if (!string.IsNullOrWhiteSpace(postImage.MediaPath))
        {
            return $"{Request.PathBase}/{postImage.MediaPath.Replace('\\', '/')}";
        }

        return $"{Request.PathBase}/api/posts/GetPostMedia/{postImage.Id}";
    }

    private string GetPhysicalMediaPath(string relativeMediaPath)
    {
        var normalizedRelativePath = relativeMediaPath.Replace('/', Path.DirectorySeparatorChar)
            .Replace('\\', Path.DirectorySeparatorChar);

        return Path.Combine(_webHostEnvironment.WebRootPath, normalizedRelativePath);
    }

}
