using NHASoftware.DBContext;
using NHASoftware.Services.Anime;
using NHASoftware.Services.Forums;
using NHASoftware.Services.Social;

namespace NHASoftware.Services.RepositoryPatternFoundationals
{
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



        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            ForumSectionRepository = new ForumSectionRepository(_context);
            ForumTopicRepository = new ForumTopicRepository(_context);
            ForumPostRepository = new ForumPostRepository(_context);
            ForumCommentRepository = new ForumCommentRepository(_context);
            AnimePageRepository = new AnimePageRepository(_context);
            PostRepository = new PostRepository(_context);
            UserLikeRepository = new UserLikeRepository(_context);
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async void Dispose()
        {
            await _context.DisposeAsync();
        }
    }
}
