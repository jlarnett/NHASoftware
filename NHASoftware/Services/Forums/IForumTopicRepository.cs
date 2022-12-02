using NHASoftware.Entities.Forums;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHASoftware.Services.Forums
{
    public interface IForumTopicRepository : IGenericRepository<ForumTopic>
    {
        public Task<List<ForumPost>> GetForumTopicPostsAsync(int? forumTopicId);
    }
}
