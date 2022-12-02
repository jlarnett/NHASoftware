using NHASoftware.Entities.Forums;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHASoftware.Services.Forums
{
    public interface IForumPostRepository : IGenericRepository<ForumPost>
    {
        public Task<ForumPost> GetForumPostWithLazyLoadingAsync(int? forumPostId);
    }
}
