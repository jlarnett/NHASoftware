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
        return await _context.PostImages!
            .AsNoTracking()
            .Where(postImage => postImage.PostId == postId)
            .OrderBy(postImage => postImage.Id)
            .Select(postImage => new PostImage
            {
                Id = postImage.Id,
                PostId = postImage.PostId,
                MediaPath = postImage.MediaPath,
                FileExtensionType = postImage.FileExtensionType,
                ImageBytes = postImage.FileExtensionType.StartsWith(".mp4")
                    || postImage.FileExtensionType.StartsWith(".webm")
                    || postImage.FileExtensionType.StartsWith(".ogg")
                    || postImage.FileExtensionType.StartsWith(".mov")
                        ? null
                        : postImage.ImageBytes
            })
            .ToListAsync();
    }

    public async Task<PostImage?> GetPostMediaAsync(int? mediaId)
    {
        return await _context.PostImages!
            .AsNoTracking()
            .Where(postImage => postImage.Id == mediaId)
            .Select(postImage => new PostImage
            {
                Id = postImage.Id,
                FileExtensionType = postImage.FileExtensionType,
                MediaPath = postImage.MediaPath,
                ImageBytes = postImage.ImageBytes
            })
            .FirstOrDefaultAsync();
    }

    public async Task<bool> HasImagesAttachedAsync(int? postId)
    {
        return await _context.PostImages!
            .AsNoTracking()
            .AnyAsync(postImage => postImage.PostId == postId);
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
