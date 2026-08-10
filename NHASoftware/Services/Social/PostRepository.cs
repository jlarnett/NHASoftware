using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Social_Entities;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Services.Social;
public class PostRepository : GenericRepository<Post>, IPostRepository
{
    public PostRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Accesses the EF context & gets all social media post. DOES NOT INCLUDE POST WITH ISDELETEDFLAG set to true
    /// </summary>
    /// <returns></returns>
    public async Task<List<Post>> GetAllPostsWithIncludesAsync() => await _context.Posts!
            .Include(p => p.User)
            .Include(p => p.ParentPost)
            .Where(p => p.IsDeletedFlag.Equals(false))
            .OrderByDescending(p => p.CreationDate)
            .AsNoTracking()
            .ToListAsync();

    public async Task<List<Post>> GetParentPostsWithIncludesAsync(IEnumerable<int> hiddenPostIds, int pageNumber = 1, int pageSize = 10)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var hiddenPostIdList = hiddenPostIds.Distinct().ToList();
        var query = _context.Posts!
            .Include(p => p.User)
            .Where(p => !p.IsDeletedFlag && p.ParentPostId == null);

        if (hiddenPostIdList.Count > 0)
        {
            query = query.Where(p => p.Id.HasValue && !hiddenPostIdList.Contains(p.Id.Value));
        }

        return await query
            .OrderByDescending(p => p.CreationDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Post?> GetPostByIDWithIncludesAsync(int postId)
    {
        return await _context.Posts!.Include(p => p.User).FirstOrDefaultAsync(p => p.Id.Equals(postId));
    }

    public async Task<Dictionary<int, int>> GetCommentCountsByParentPostIdsAsync(IEnumerable<int> parentPostIds)
    {
        var parentPostIdList = parentPostIds.Distinct().ToList();

        if (parentPostIdList.Count == 0)
        {
            return [];
        }

        return await _context.Posts!
            .AsNoTracking()
            .Where(post => post.ParentPostId.HasValue
                && parentPostIdList.Contains(post.ParentPostId.Value)
                && !post.IsHiddenFromUserProfile
                && !post.IsDeletedFlag)
            .GroupBy(post => post.ParentPostId!.Value)
            .ToDictionaryAsync(group => group.Key, group => group.Count());
    }

    /// <summary>
    /// Accesses the EF context & gets all social posts for specified users. 
    /// </summary>
    /// <param name="userId">userId of the post you want to pull from DB</param>
    /// <returns>List of social media posts. </returns>
    public async Task<List<Post>> GetUsersSocialPostsAsync(string userId)
    {
        return await _context.Posts!
            .Include(p => p.User)
            .Include(p => p.ParentPost)
            .Where(u => u.UserId!.Equals(userId) && u.IsDeletedFlag.Equals(false) && u.IsHiddenFromUserProfile.Equals(false))
            .OrderByDescending(p => p.CreationDate)
            .AsNoTracking()
            .ToListAsync();
    }
}
