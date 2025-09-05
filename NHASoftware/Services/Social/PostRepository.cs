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

    public async Task<Post?> GetPostByIDWithIncludesAsync(int postId)
    {
        return await _context.Posts!.Include(p => p.User).Include(p => p.ParentPost)
            .FirstOrDefaultAsync(p => p.Id.Equals(postId));
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
