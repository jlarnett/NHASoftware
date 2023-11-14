using NHA.Website.Software.Services.Anime;
using NHA.Website.Software.Services.Forums;
using NHA.Website.Software.Services.FriendSystem;
using NHA.Website.Software.Services.Social;

namespace NHA.Website.Software.Services.RepositoryPatternFoundationals
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
