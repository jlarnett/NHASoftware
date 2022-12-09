using NHASoftware.Services.Anime;
using NHASoftware.Services.Forums;

namespace NHASoftware.Services.RepositoryPatternFoundationals
{
    public interface IUnitOfWork
    {
        IForumSectionRepository ForumSectionRepository { get; }
        IForumTopicRepository ForumTopicRepository { get; }
        IForumPostRepository ForumPostRepository { get; }
        IForumCommentRepository ForumCommentRepository { get; }
        IAnimePageRepository AnimePageRepository { get; }
        Task<int> CompleteAsync();
    }
}
