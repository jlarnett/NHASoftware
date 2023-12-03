using NHA.Website.Software.ConsumableEntities.DTOs;
using NHA.Website.Software.Entities.Social_Entities;

namespace NHA.Website.Software.Services.Social.PostBuilderService
{
    public interface IPostBuilder
    {
        /// <summary>
        /// Retrieves list of post comments for specified postId.
        /// Returns list of PostDTO with details fully populated for use. 
        /// </summary>
        /// <param name="id">PostId to obtain comments for</param>
        /// <returns>List of comment PostDTOs</returns>
        Task<List<PostDTO>> FindPostChildren(int? id);

        /// <summary>
        /// Gets all social posts for supplied userId. Populates the PostDTO like details. 
        /// </summary>
        /// <param name="userId">Users Identity Id you want posts for</param>
        /// <returns>postsDto IEnumerable</returns>
        Task<List<PostDTO>> GetAllPostForUser(string userId);

        /// <summary>
        /// Retrieves a list of all parent posts in DB. Fully populates the PostDTOs & handles caching 
        /// </summary>
        /// <returns>a list of all parent posts in DB in PostDTO format</returns>
        Task<List<PostDTO>> RetrieveParentPosts();


        /// <summary>
        /// Populates PostDTO with currently logged in users like details (whether they liked the post & how many people like post in general)
        /// Also sets a HasImageAttached flag. This triggers dynamic image loading from JS
        /// </summary>
        /// <param name="dto">The postDTO to finish populating</param>
        /// <returns>Fully populated postDTO</returns>
        Task<PostDTO> PopulatePostDTO(PostDTO postDTO);

        /// <summary>
        /// Gets the total number of comments for supplied postId.
        /// </summary>
        /// <param name="postId">postId to retrieve # of comments for</param>
        /// <returns>integer number of comments</returns>
        Task<int> GetCommentCountForPost(int? postId);

        Task<PostDTO> LocateNewlyCreatedPost(Post post);
    }
}