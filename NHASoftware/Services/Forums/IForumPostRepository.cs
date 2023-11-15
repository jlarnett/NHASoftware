using NHA.Website.Software.Entities.Forums;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Services.Forums;
public interface IForumPostRepository : IGenericRepository<ForumPost>
{
    public Task<ForumPost?> GetForumPostWithLazyLoadingAsync(int? forumPostId);
}
