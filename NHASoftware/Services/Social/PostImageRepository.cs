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

    public async Task<HashSet<int>> GetPostIdsWithImagesAsync(IEnumerable<int> postIds)
    {
        var postIdsList = postIds.Distinct().ToList();

        if (postIdsList.Count == 0)
        {
            return [];
        }

        var result = await _context.PostImages!
            .AsNoTracking()
            .Where(postImage => postImage.PostId.HasValue && postIdsList.Contains(postImage.PostId.Value))
            .Select(postImage => postImage.PostId!.Value)
            .Distinct()
            .ToListAsync();

        return [.. result];
    }
}
