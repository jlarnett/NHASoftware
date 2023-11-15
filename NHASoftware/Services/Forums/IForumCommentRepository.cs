using NHA.Website.Software.Entities.Forums;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Services.Forums;
public interface IForumCommentRepository : IGenericRepository<ForumComment>
{
    public Task<List<ForumComment>> GetForumPostCommentsAsync(int? forumPostId);
    public Task<int> GetNumberOfCommentsForPost(int? forumPostId);
    public Task<ForumComment?> GetForumCommentWithLazyLoadingAsync(int? forumCommentId);
}
