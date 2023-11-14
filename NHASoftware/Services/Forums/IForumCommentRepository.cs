using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHASoftware.Entities.Forums;

namespace NHA.Website.Software.Services.Forums
{
    public interface IForumCommentRepository : IGenericRepository<ForumComment>
    {
        public Task<List<ForumComment>> GetForumPostCommentsAsync(int? forumPostId);
        public Task<int> GetNumberOfCommentsForPost(int? forumPostId);
        public Task<ForumComment?> GetForumCommentWithLazyLoadingAsync(int? forumCommentId);
    }
}
