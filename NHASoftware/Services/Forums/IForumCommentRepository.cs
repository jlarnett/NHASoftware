using NHASoftware.Entities.Forums;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHASoftware.Services.Forums
{
    public interface IForumCommentRepository : IGenericRepository<ForumComment>
    {
        public Task<List<ForumComment>> GetForumPostCommentsAsync(int? forumPostId);
        public Task<int> GetNumberOfCommentsForPost(int? forumPostId);
        public Task<ForumComment> GetForumCommentWithLazyLoadingAsync(int? forumCommentId);
    }
}
