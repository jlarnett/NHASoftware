using NHASoftware.Models.ForumModels;

namespace NHASoftware.Services
{
    public interface IForumRepository
    {
        Task<List<ForumComment>> GetForumCommentsAsync();
        Task<ForumPost> GetForumPostDetails(int? forumPostId);
        Task<List<ForumPost>> GetForumPostsAsync();
        Task<List<ForumSection>> GetForumSectionsAsync();
        Task<ForumTopic> GetForumTopic(int? forumTopicId);
        Task<List<ForumPost>> GetForumTopicPosts(int? forumTopicId);
        Task<List<ForumTopic>> GetForumTopicsAsync();
    }
}