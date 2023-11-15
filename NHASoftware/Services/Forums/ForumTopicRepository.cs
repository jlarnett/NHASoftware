using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Forums;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Services.Forums;
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
