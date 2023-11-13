using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHASoftware.Entities.Forums;

namespace NHA.Website.Software.Services.Forums
{
    public interface IForumTopicRepository : IGenericRepository<ForumTopic>
    {
        public Task<List<ForumPost>> GetForumTopicPostsAsync(int? forumTopicId);
    }
}
