using NHASoftware.Models.ForumModels;

namespace NHASoftware.Services
{
    public interface IForumRepository
    {
        Task<List<ForumComment>> GetForumCommentsAsync();
        Task<ForumPost> GetForumPostAsync(int? forumPostId);
        Task<List<ForumComment>> GetForumPostCommentsAsync(int? forumPostId);
        Task<List<ForumPost>> GetForumPostsAsync();
        Task<ForumSection> GetForumSectionAsync(int? forumSectionId);
        Task<List<ForumSection>> GetForumSectionsAsync();
        Task<ForumTopic> GetForumTopicAsync(int? forumTopicId);
        Task<List<ForumPost>> GetForumTopicPostsAsync(int? forumTopicId);
        Task<List<ForumTopic>> GetForumTopicsAsync();
    }
}