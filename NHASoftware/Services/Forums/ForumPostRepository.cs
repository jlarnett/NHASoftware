using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHASoftware.DBContext;
using NHASoftware.Entities.Forums;

namespace NHA.Website.Software.Services.Forums
{
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
}
