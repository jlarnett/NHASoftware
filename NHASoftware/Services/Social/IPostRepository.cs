using NHA.Website.Software.Entities.Social_Entities;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Services.Social;
public interface IPostRepository : IGenericRepository<Post>
{
    public Task<List<Post>> GetAllPostsWithIncludesAsync();
    public Task<List<Post>> GetParentPostsWithIncludesAsync(IEnumerable<int> hiddenPostIds, int pageNumber = 1, int pageSize = 10);
    public Task<List<Post>> GetUsersSocialPostsAsync(string userId);
    public Task<Post?> GetPostByIDWithIncludesAsync(int postId);
    public Task<Dictionary<int, int>> GetCommentCountsByParentPostIdsAsync(IEnumerable<int> parentPostIds);
}
