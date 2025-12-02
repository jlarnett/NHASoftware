using NHA.Website.Software.DBContext;
using NHA.Website.Software.Services.Anime;
using NHA.Website.Software.Services.ChatSystem;
using NHA.Website.Software.Services.Forums;
using NHA.Website.Software.Services.FriendSystem;
using NHA.Website.Software.Services.Game;
using NHA.Website.Software.Services.SessionHistory;
using NHA.Website.Software.Services.Social;
using NHA.Website.Software.Services.Sponsors;

namespace NHA.Website.Software.Services.RepositoryPatternFoundationals;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public IForumSectionRepository ForumSectionRepository { get; private set; }
    public IForumTopicRepository ForumTopicRepository { get; private set; }
    public IForumPostRepository ForumPostRepository { get; private set; }
    public IForumCommentRepository ForumCommentRepository { get; private set; }
    public IAnimePageRepository AnimePageRepository { get; private set; }
    public IPostRepository PostRepository { get; private set; }
    public IUserLikeRepository UserLikeRepository { get; private set; }
    public IFriendRequestRepository FriendRequestRepository { get; set; }
    public IFriendRepository FriendRepository { get; set; }
    public IPostImageRepository PostImageRepository { get; set; }
    public ISessionHistoryRepository SessionHistoryRepository { get; set; }
    public IChatMessageRepository ChatMessageRepository { get; set; }
    public ISponsorAdRepository SponsorAdRepository { get; set; }
    public IGamePageRepository GamePageRepository { get; set; }
    public IHiddenPostRepository HiddenPostRepository { get; set; }


    private readonly ILogger<UnitOfWork> _logger;

    public UnitOfWork(ApplicationDbContext context, ILogger<UnitOfWork> logger)
    {
        _context = context;
        ForumSectionRepository = new ForumSectionRepository(_context);
        ForumTopicRepository = new ForumTopicRepository(_context);
        ForumPostRepository = new ForumPostRepository(_context);
        ForumCommentRepository = new ForumCommentRepository(_context);
        AnimePageRepository = new AnimePageRepository(_context);
        PostRepository = new PostRepository(_context);
        UserLikeRepository = new UserLikeRepository(_context);
        FriendRequestRepository = new FriendRequestRepository(_context);
        FriendRepository = new FriendRepository(_context);
        PostImageRepository = new PostImageRepository(_context);
        SessionHistoryRepository = new SessionHistoryRepository(_context);
        ChatMessageRepository = new ChatMessageRepository(_context);
        SponsorAdRepository = new SponsorAdRepository(_context);
        GamePageRepository = new GamePageRepository(_context);
        HiddenPostRepository = new HiddenPostRepository(_context);
        
        _logger = logger;
    }

    /// <summary>
    /// Saves all changes for unit of work service.
    /// </summary>
    /// <returns></returns>
    public async Task<int> CompleteAsync()
    {
        try
        {
            return await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return 0;
        }
    }
    
    /// <summary>
    /// Dispose of the ApplicationDBContext
    /// </summary>
    public async void Dispose()
    {
        await _context.DisposeAsync();
    }
}
