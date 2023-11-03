using NHA.Website.Software.Services.Social;
using NHASoftware.Services.Anime;
using NHASoftware.Services.Forums;
using NHASoftware.Services.FriendSystem;

namespace NHASoftware.Services.RepositoryPatternFoundationals
{
    public interface IUnitOfWork
    {
        IForumSectionRepository ForumSectionRepository { get; }
        IForumTopicRepository ForumTopicRepository { get; }
        IForumPostRepository ForumPostRepository { get; }
        IForumCommentRepository ForumCommentRepository { get; }
        IAnimePageRepository AnimePageRepository { get; }
        IPostRepository PostRepository { get; }
        IUserLikeRepository UserLikeRepository { get; }
        IFriendRequestRepository FriendRequestRepository { get; }
        IFriendRepository FriendRepository { get; }
        IPostImageRepository PostImageRepository { get; }
        Task<int> CompleteAsync();
    }
}
