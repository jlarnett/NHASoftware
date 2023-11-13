using NHA.Website.Software.Entities.Social_Entities;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;

namespace NHA.Website.Software.Services.Social
{
    public interface IPostImageRepository : IGenericRepository<PostImage>
    {
        public Task<bool> HasImagesAttachedAsync(int? postId);
        public Task<List<PostImage>> GetPostImagesAsync(int? postId);
    }
}
