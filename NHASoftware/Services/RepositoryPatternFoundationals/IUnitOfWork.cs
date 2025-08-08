using NHA.Website.Software.Services.Anime;
using NHA.Website.Software.Services.ChatSystem;
using NHA.Website.Software.Services.Forums;
using NHA.Website.Software.Services.FriendSystem;
using NHA.Website.Software.Services.Game;
using NHA.Website.Software.Services.SessionHistory;
using NHA.Website.Software.Services.Social;
using NHA.Website.Software.Services.Sponsors;

namespace NHA.Website.Software.Services.RepositoryPatternFoundationals;
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
    ISessionHistoryRepository SessionHistoryRepository { get; }
    IChatMessageRepository ChatMessageRepository { get; }
    ISponsorAdRepository SponsorAdRepository { get; }
    IGamePageRepository GamePageRepository { get; }
    Task<int> CompleteAsync();
}
