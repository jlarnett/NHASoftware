using AutoMapper.Configuration;
using Microsoft.EntityFrameworkCore;
using NHASoftware.DBContext;
using NHASoftware.Entities.Forums;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHASoftware.Services.Forums
{
    public class ForumPostRepository : GenericRepository<ForumPost>, IForumPostRepository
    {
        public ForumPostRepository(ApplicationDbContext context):base(context)
        {
            
        }

        public async Task<ForumPost> GetForumPostWithLazyLoadingAsync(int? forumPostId)
        {
            return await _context.ForumPosts
                .Include(f => f.ForumTopic)
                .Include(f => f.User)
                .FirstOrDefaultAsync(m => m.Id == forumPostId);
        }
    }
}
