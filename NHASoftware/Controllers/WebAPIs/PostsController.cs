using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.FeatureManagement.Mvc;
using NHA.Helpers.ImageDataSourceTranslator;
using NHA.Website.Software.Caching;
using NHA.Website.Software.Entities.Social_Entities;
using NHA.Website.Software.Services.CacheLoadingManager;
using NHA.Website.Software.Services.FileExtensionValidator;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHASoftware.ConsumableEntities.DTOs;
using NHASoftware.Entities.Identity;
using NHASoftware.Entities.Social_Entities;

namespace NHA.Website.Software.Controllers.WebAPIs
{
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


        public PostsController(IMapper mapper, IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager,
            ILogger<PostDTO> logger, IFileExtensionValidator validator, IImageDataSourceTranslator imageTranslator, IMemoryCache memoryCache, ICacheLoadingManager cacheLoadingManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _logger = logger;
            _fileExtensionValidator = validator;
            _imageDataSourceTranslator = imageTranslator;
            _memoryCache = memoryCache;
            _cacheLoadingManager = cacheLoadingManager;
        }

        /// <summary>
        /// GET: api/Post
        /// calls the PostRepository and gets all social media posts in DB. DOES NOT INCLUDE DELETE POST. Seperate endpoint for deleted post. 
        /// </summary>
        /// <returns>IEnumerable of all posts.</returns>
        [HttpGet]
        public async Task<List<PostDTO>> GetPosts()
        {
            var shouldReloadCache = _cacheLoadingManager.ShouldCacheReload(CachingKeys.Posts);

            if (!_memoryCache.TryGetValue(CachingKeys.PopulatedPostDTOs, out List<PostDTO>? populatedPostDTOs) || shouldReloadCache)
            {
                populatedPostDTOs = await GeneratePostDTOList();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

                _memoryCache.Set(CachingKeys.PopulatedPostDTOs, populatedPostDTOs, cacheEntryOptions);
            }

            return populatedPostDTOs!;
        }

        /// <summary>
        /// Gets all social posts for supplied userId. Populates the PostDTO like details. 
        /// </summary>
        /// <param name="userId">Users Identity Id you want posts for</param>
        /// <returns>postsDto IEnumerable </returns>
        [HttpGet("GetSocialPostForUserId/{userId}")]
        public async Task<IEnumerable<PostDTO>> GetAllPostForUserId(string userId)
        {
            var posts = await _unitOfWork.PostRepository.GetUsersSocialPostsAsync(userId);
            var postsDtos = posts.Select(_mapper.Map<Post, PostDTO>).ToList();
            return await PopulatePostDTODetails(postsDtos);
        }

        // GET: api/Posts/5
        [HttpGet("GetSocialPosts/{id}")]
        public async Task<ActionResult<Post>> GetPost(int? id)
        {
            var post = await _unitOfWork.PostRepository.GetByIdAsync(id);

            if (post == null)
            {
                return NotFound();
            }

            return post;
        }

        [HttpGet("FindChildrenPosts/{id}")]
        public async Task<List<PostDTO>> FindChildrenPosts(int? id)
        {
            var posts = await _unitOfWork.PostRepository.GetAllPostsWithIncludesAsync();
            var childrenPosts = posts.Where(p => p.ParentPostId == id && p.IsHiddenFromUserProfile == false).ToList();

            var postsDtos = childrenPosts.Select(_mapper.Map<Post, PostDTO>).ToList();
            return await PopulatePostDTODetails(postsDtos);
        }

        [HttpGet("GetUserSocialPosts/{id}")]
        public async Task<List<PostDTO>> GetUsersSocialPosts(string id)
        {
            var posts = await _unitOfWork.PostRepository.GetUsersSocialPostsAsync(id);
            return posts.Select(_mapper.Map<Post, PostDTO>).ToList();
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
                    _imageDataSourceTranslator.GetDataSourceTranslation(image.FileExtensionType, image.ImageBytes);
                imageDataSources.Add(imageDataSource);
            }

            return Ok(imageDataSources);
        }

        private async Task<List<PostDTO>> GeneratePostDTOList()
        {
            var posts = await _unitOfWork.PostRepository.GetAllPostsWithIncludesAsync();
            var postDTOs = posts.Select(_mapper.Map<Post, PostDTO>).ToList();
            return await PopulatePostDTODetails(postDTOs);
        }

        /// <summary>
        /// Goes through the entire list of post & repopulates the postDtos with like counters. 
        /// </summary>
        /// <param name="postDtos">List of postsDto objects.</param>
        /// <returns>IEnumerable list of postDtos with the like counters populated. </returns>
        private async Task<List<PostDTO>> PopulatePostDTODetails(List<PostDTO> postDtos)
        {
            List<PostDTO> posts = new List<PostDTO>();
            foreach (var dto in postDtos)
            {
                posts.Add(await PopulatePostDTO(dto));
            }
            return posts;
        }

        private async Task<PostDTO> PopulatePostDTO(PostDTO dto)
        {
            dto.LikeCount = (await _unitOfWork.UserLikeRepository.FindAsync(p => p.PostId == dto.Id && !p.IsDislike)).Count();
            dto.DislikeCount = (await _unitOfWork.UserLikeRepository.FindAsync(p => p.PostId == dto.Id && p.IsDislike)).Count();
            dto.UserLikedPost = await UserLikedPost(dto.Id);
            dto.UserDislikedPost = await UserDislikedPost(dto.Id);
            dto.HasImagesAttached = await _unitOfWork.PostImageRepository.HasImagesAttachedAsync(dto.Id);

            return dto;
        }

        /// <summary>
        /// Checks whether the current user has liked the specified Post Id. 
        /// </summary>
        /// <param name="id">Post Id</param>
        /// <returns>Returns boolean condition if user liked post id</returns>
        private async Task<bool> UserLikedPost(int? id)
        {
            return (await _unitOfWork.UserLikeRepository.FindAsync(ul =>
                ul.PostId == id && ul.IsDislike == false && ul.UserId == _userManager.GetUserId(User))).Any();
        }

        /// <summary>
        /// Checks whether the current user has Disliked the specified Post Id. 
        /// </summary>
        /// <param name="id">Post Id</param>
        /// <returns>Returns boolean condition if user Disliked post id</returns>
        private async Task<bool> UserDislikedPost(int? id)
        {
            return (await _unitOfWork.UserLikeRepository.FindAsync(ul =>
                ul.PostId == id && ul.IsDislike == true && ul.UserId == _userManager.GetUserId(User))).Any();
        }


        // PUT: api/Posts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> PostPost([Bind("Summary,UserId,ParentPostId")] PostDTO postdto)
        {
            var post = _mapper.Map<PostDTO, Post>(postdto);
            post.CreationDate = DateTime.Now;

            _unitOfWork.PostRepository.Add(post);
            var result = await _unitOfWork.CompleteAsync();

            if (result > 0)
            {
                var newPost = _unitOfWork.PostRepository.Find(p =>
                    p.Summary.Equals(postdto.Summary) && p.UserId!.Equals(postdto.UserId)).FirstOrDefault();

                //Populating the postDto
                if (newPost != null)
                {
                    var newPostWithIncludes = await _unitOfWork.PostRepository.GetPostByIDWithIncludesAsync(newPost.Id.GetValueOrDefault());
                    var postDto = PopulatePostDTO(_mapper.Map<Post, PostDTO>(newPostWithIncludes!));
                    _logger.Log(LogLevel.Information, "Post API successfully added new post to DB {post}", postDto);

                    _cacheLoadingManager.IncrementCacheChangeCounter(CachingKeys.Posts);
                    return Ok(new { success = true, data = postDto });
                }
                else
                {
                    return BadRequest(new { success = false });
                }
            }
            else
            {
                _logger.Log(LogLevel.Debug, "system was unable to add postDto to DB.");
                return BadRequest(new { success = false });
            }
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
        public async Task<IActionResult> PostCustomizedPost([FromForm] PostDTO postdto)
        {
            var imageFilesIncluded = postdto.ImageFiles != null && postdto.ImageFiles.Count > 0;

            if (imageFilesIncluded)
            {
                foreach (var imageFile in postdto.ImageFiles!)
                {
                    if (!_fileExtensionValidator.CheckValidImageExtensions(imageFile.FileName))
                        return BadRequest(new
                        { success = false, message = "Unable To Submit Custom Post - File is Not Image Extension" });
                }
            }

            var post = _mapper.Map<PostDTO, Post>(postdto);
            post.CreationDate = DateTime.Now;
            post.UserId = _userManager.GetUserId(User);

            _unitOfWork.PostRepository.Add(post);
            var result = await _unitOfWork.CompleteAsync();

            if (result > 0)
            {
                var newPost = _unitOfWork.PostRepository.Find(p =>
                    p.Summary.Equals(postdto.Summary) && p.UserId!.Equals(postdto.UserId)).FirstOrDefault();

                //Populating the postDto
                if (newPost != null)
                {
                    if (imageFilesIncluded)
                    {
                        var imageSaveResult = await SavePostImagesToDatabase(newPost.Id, postdto.ImageFiles!);

                        if (!imageSaveResult)
                        {
                            return BadRequest(new { success = false, message = "Error Saving Images To DB" });
                        }
                    }
                    var newPostWithIncludes = await _unitOfWork.PostRepository.GetPostByIDWithIncludesAsync(newPost.Id.GetValueOrDefault());
                    var postDto = PopulatePostDTO(_mapper.Map<Post, PostDTO>(newPostWithIncludes));
                    _logger.Log(LogLevel.Information, "Post API successfully added new post to DB {post}", postDto);
                    _cacheLoadingManager.IncrementCacheChangeCounter(CachingKeys.Posts);
                    return Ok(new { success = true, data = postDto });
                }
                else
                {
                    return BadRequest(new { success = false });
                }
            }
            else
            {
                _logger.Log(LogLevel.Debug, "system was unable to add postDto to DB.");
                return BadRequest(new { success = false });
            }
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
            _logger.Log(LogLevel.Information, "attempted to execute - {0}, parameters - {1}", nameof(DeletePost), HttpContext.Request.Body);

            if (post == null)
            {
                _logger.Log(LogLevel.Debug, "Failed to execute {0} post id not found", nameof(DeletePost));
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

        private async Task<bool> SavePostImagesToDatabase(int? postId, List<IFormFile> ImageFiles)
        {
            List<PostImage> images = new List<PostImage>();

            foreach (var imageFile in ImageFiles)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(memoryStream);

                    // Upload the file if less than 5 MB
                    if (memoryStream.Length < 5242880)
                    {
                        PostImage postImage = new PostImage()
                        {
                            ImageBytes = memoryStream.ToArray(),
                            PostId = postId,
                            FileExtensionType = Path.GetExtension(imageFile.FileName)
                        };

                        images.Add(postImage);
                    }
                }

            }

            _unitOfWork.PostImageRepository.AddRange(images);
            var savePostImageResult = await _unitOfWork.CompleteAsync();

            if (savePostImageResult > 0)
            {
                return true;
            }

            return false;
        }

    }
}
