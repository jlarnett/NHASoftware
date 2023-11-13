using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHASoftware.DBContext;
using NHASoftware.Entities.Forums;

namespace NHA.Website.Software.Services.Forums
{
    public class ForumTopicRepository : GenericRepository<ForumTopic>, IForumTopicRepository
    {
        public ForumTopicRepository(ApplicationDbContext context) : base(context)
        {

        }

        public async Task<List<ForumPost>> GetForumTopicPostsAsync(int? forumTopicId)
        {
            return await _context.ForumPosts!.Where(c => c.ForumTopicId == forumTopicId).ToListAsync();
        }
    }
}
