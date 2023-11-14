using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHASoftware.Entities.Forums;

namespace NHA.Website.Software.Services.Forums
{
    public interface IForumPostRepository : IGenericRepository<ForumPost>
    {
        public Task<ForumPost?> GetForumPostWithLazyLoadingAsync(int? forumPostId);
    }
}
