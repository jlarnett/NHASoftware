using NHA.Website.Software.Entities.Forums;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Services.Forums;
public interface IForumTopicRepository : IGenericRepository<ForumTopic>
{
    public Task<List<ForumPost>> GetForumTopicPostsAsync(int? forumTopicId);
}
