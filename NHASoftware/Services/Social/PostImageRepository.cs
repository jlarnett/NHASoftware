using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Social_Entities;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Services.Social;
public class PostImageRepository : GenericRepository<PostImage>, IPostImageRepository
{
    public PostImageRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<List<PostImage>> GetPostImagesAsync(int? postId)
    {
        return await _context.PostImages!.FromSql($"GetPostImages {postId}").ToListAsync();
        //return await _context.PostImages!.Where(pi => pi.PostId.Equals(postId)).ToListAsync();
    }

    public async Task<bool> HasImagesAttachedAsync(int? postId)
    {
        var result = await _context.PostImages!.FromSql($"CheckImagesExistForPost {postId}").ToListAsync();
        var firstPost = result.FirstOrDefault();
        return firstPost != null;
    }
}
