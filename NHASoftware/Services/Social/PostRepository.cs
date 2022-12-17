using Microsoft.EntityFrameworkCore;
using NHASoftware.DBContext;
using NHASoftware.Entities.Social_Entities;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHASoftware.Services.Social
{
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        public PostRepository(ApplicationDbContext context) : base(context)
        {
        }

        public void AddUsingSproc(Post post)
        {
            var newPost = _context.Posts.FromSqlRaw(
                $"posts_InsertPostData {post.Summary.ToString()}, {post.CreationDate}, {post.UserId}, {null}");
        }

        public async Task<List<Post>> GetAllPostsWithIncludesAsync()
        {
            return await _context.Posts
                .Include(p => p.User)
                .Include(p => p.ParentPost)
                .ToListAsync();
        }
    }
}
