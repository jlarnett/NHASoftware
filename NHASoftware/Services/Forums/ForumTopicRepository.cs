using Microsoft.EntityFrameworkCore;
using NHASoftware.DBContext;
using NHASoftware.Entities.Forums;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHASoftware.Services.Forums
{
    public class ForumTopicRepository : GenericRepository<ForumTopic>, IForumTopicRepository
    {
        public ForumTopicRepository(ApplicationDbContext context):base(context)
        {
            
        }

        public async Task<List<ForumPost>> GetForumTopicPostsAsync(int? forumTopicId)
        {
            return await _context.ForumPosts.Where(c => c.ForumTopicId == forumTopicId).ToListAsync();
        }
    }
}
