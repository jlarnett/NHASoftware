using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Forums;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Services.Forums;
public class ForumPostRepository : GenericRepository<ForumPost>, IForumPostRepository
{
    public ForumPostRepository(ApplicationDbContext context) : base(context)
    {

    }

    public async Task<ForumPost?> GetForumPostWithLazyLoadingAsync(int? forumPostId)
    {
        return await _context.ForumPosts!
            .Include(f => f.ForumTopic)
            .Include(f => f.User)
            .FirstOrDefaultAsync(m => m.Id == forumPostId);
    }
}
