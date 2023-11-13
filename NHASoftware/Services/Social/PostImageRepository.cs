using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.Entities.Social_Entities;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHASoftware.DBContext;

namespace NHA.Website.Software.Services.Social
{
    public class PostImageRepository : GenericRepository<PostImage>, IPostImageRepository
    {
        public PostImageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<PostImage>> GetPostImagesAsync(int? postId)
        {
            return await _context.PostImages!.Where(pi => pi.PostId.Equals(postId)).ToListAsync();
        }

        public async Task<bool> HasImagesAttachedAsync(int? postId)
        {
            return await _context.PostImages!.AnyAsync(pi => pi.PostId.Equals(postId));
        }
    }
}
