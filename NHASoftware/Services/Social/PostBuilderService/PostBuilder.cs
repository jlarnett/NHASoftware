using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using NHA.Website.Software.Caching;
using NHA.Website.Software.ConsumableEntities.DTOs;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Entities.Social_Entities;
using NHA.Website.Software.Services.CacheLoadingManager;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using System.Security.Claims;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace NHA.Website.Software.Services.Social.PostBuilderService;

public class PostBuilder : IPostBuilder
{
    private readonly ICacheLoadingManager _cacheLoadingManager;
    private readonly IMemoryCache _memoryCache;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;

    public PostBuilder(ICacheLoadingManager cacheLoadingManager, IMemoryCache memoryCache, IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
    {
        _cacheLoadingManager = cacheLoadingManager;
        _memoryCache = memoryCache;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userManager = userManager;
    }

    /// <summary>
    /// Retrieves a list of all parent posts in DB. Fully populates the PostDTOs & handles caching 
    /// </summary>
    /// <returns>a list of all parent posts in DB in PostDTO format</returns>
    public async Task<List<PostDTO>> RetrieveParentPosts(string currentUserId)
    {
        var shouldReloadCache = _cacheLoadingManager.ShouldCacheReload(CachingKeys.Posts);
        var cacheKey = $"{CachingKeys.PopulatedPostDTOs}_{currentUserId}";

        if (!_memoryCache.TryGetValue(cacheKey, out List<PostDTO>? populatedPostDTOs) || shouldReloadCache)
        {
            populatedPostDTOs = await GeneratePostDTOList(await RetrieveParentPostFromRepository());
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

            _memoryCache.Set(CachingKeys.PopulatedPostDTOs, populatedPostDTOs, cacheEntryOptions);
        }

        return populatedPostDTOs!;
    }

    /// <summary>
    /// Retrieves list of post comments for specified postId.
    /// Returns list of PostDTO with details fully populated for use. 
    /// </summary>
    /// <param name="id">PostId to obtain comments for</param>
    /// <returns>List of comment PostDTOs</returns>
    public async Task<List<PostDTO>> FindPostChildren(int? id)
    {
        var posts = await _unitOfWork.PostRepository.GetAllPostsWithIncludesAsync();
        var childrenPosts = posts.Where(p => p.ParentPostId == id && p.IsHiddenFromUserProfile == false).ToList();
        var postsDTOs = childrenPosts.Select(_mapper.Map<Post, PostDTO>).ToList();
        return await PopulatePostDTODetails(postsDTOs);
    }

    /// <summary>
    /// Gets all social posts for supplied userId. Populates the PostDTO like details. 
    /// </summary>
    /// <param name="userId">Users Identity Id you want posts for</param>
    /// <returns>postsDto IEnumerable</returns>
    public async Task<List<PostDTO>> GetAllPostForUser(string userId)
    {
        var posts = await _unitOfWork.PostRepository.GetUsersSocialPostsAsync(userId);
        var postDTOs = posts.Select(_mapper.Map<Post, PostDTO>).ToList();
        return await PopulatePostDTODetails(postDTOs);
    }

    /// <summary>
    /// Gets the total number of comments for supplied postId.
    /// </summary>
    /// <param name="postId">postId to retrieve # of comments for</param>
    /// <returns>integer number of comments</returns>
    public async Task<int> GetCommentCountForPost(int? postId)
    {
        return await _unitOfWork.PostRepository.CountAsync(p =>
            p.ParentPostId == postId && p.IsHiddenFromUserProfile == false && p.IsDeletedFlag == false);
    }

    public async Task<PostDTO> LocateNewlyCreatedPost(Post post)
    {
        var newPost = (await _unitOfWork.PostRepository.FindAsync(p =>
            p.Summary.Equals(post.Summary) && p.UserId!.Equals(post.UserId) && p.CreationDate.Equals(post.CreationDate))).FirstOrDefault();

        //Populating the postDto
        if (newPost != null)
        {
            var newPostWithIncludes = await _unitOfWork.PostRepository.GetPostByIDWithIncludesAsync(newPost.Id.GetValueOrDefault());
            var postDto = await PopulatePostDto(_mapper.Map<Post, PostDTO>(newPostWithIncludes!), []);
            return postDto;
        }

        return new PostDTO();
    }

    /// <summary>
    /// Takes in IEnumerable<Post> retrieved from repository & finishes populating the postDTO objects.
    /// </summary>
    /// <param name="posts">IEnumerable<Post></param>
    /// <returns>Populated list of postDTOs ready for use</returns>
    private async Task<List<PostDTO>> GeneratePostDTOList(IEnumerable<Post> posts)
    {
        var postDTOs = posts.Select(_mapper.Map<Post, PostDTO>).ToList();
        return await PopulatePostDTODetails(postDTOs);
    }

    /// <summary>
    /// Goes through the entire list of post & repopulates the postDTOs with like counters. 
    /// </summary>
    /// <param name="postDTOs">List of postsDto objects.</param>
    /// <returns>IEnumerable list of postDTOs with the like counters populated. </returns>
    private async Task<List<PostDTO>> PopulatePostDTODetails(List<PostDTO> postDTOs)
    {
        List<PostDTO> posts = new List<PostDTO>();
        var userLikes = (await _unitOfWork.UserLikeRepository.GetAllAsync()).ToList();

        foreach (var dto in postDTOs)
        {
            posts.Add(await PopulatePostDto(dto, userLikes));
        }
        return posts;
    }

    /// <summary>
    /// Populates PostDTO with currently logged in users like details (whether they liked the post & how many people like post in general)
    /// Also sets a HasImageAttached flag. This triggers dynamic image loading from JS
    /// </summary>
    /// <param name="dto">The postDTO to finish populating</param>
    /// <returns>Fully populated postDTO</returns>
    public async Task<PostDTO> PopulatePostDto(PostDTO dto, List<UserLikes> userLikes)
    {
        dto.LikeCount = (await _unitOfWork.UserLikeRepository.FindAsync(p => p.PostId == dto.Id && !p.IsDislike)).Count();
        dto.DislikeCount = (await _unitOfWork.UserLikeRepository.FindAsync(p => p.PostId == dto.Id && p.IsDislike)).Count();
        dto.UserLikedPost = UserLikedPost(dto.Id, userLikes);
        dto.UserDislikedPost = UserDislikedPost(dto.Id, userLikes);
        dto.HasImagesAttached = await _unitOfWork.PostImageRepository.HasImagesAttachedAsync(dto.Id);

        return dto;
    }

    /// <summary>
    /// Checks whether the current user has liked the specified Post Id. 
    /// </summary>
    /// <param name="id">Post Id</param>
    /// <returns>Returns boolean condition if user liked post id</returns>
    private bool UserLikedPost(int? id, List<UserLikes> userLikes)
    {
        if (ClaimsPrincipal.Current != null)
        {
            return userLikes.Any(ul => ul.PostId == id && ul.IsDislike == false && ul.UserId == _userManager.GetUserId(ClaimsPrincipal.Current));
        }

        return false;
    }

    /// <summary>
    /// Checks whether the current user has Disliked the specified Post Id. 
    /// </summary>
    /// <param name="id">Post Id</param>
    /// <returns>Returns boolean condition if user Disliked post id</returns>
    private bool UserDislikedPost(int? id, List<UserLikes> userLikes)
    {
        if (ClaimsPrincipal.Current != null)
        {
            return userLikes.Any(ul =>
                ul.PostId == id && ul.IsDislike == true && ul.UserId == _userManager.GetUserId(ClaimsPrincipal.Current));
        }

        return false;
    }


    /// <summary>
    /// Retrieve all the parent post from PostRepository. Basically any post without ParentPostId value. 
    /// </summary>
    /// <returns>IEnumerable of posts retrieved from DB. </returns>
    private async Task<IEnumerable<Post>> RetrieveParentPostFromRepository()
    {
        return (await _unitOfWork.PostRepository.GetAllPostsWithIncludesAsync()).Where(p => p.ParentPostId == null);
    }
}
