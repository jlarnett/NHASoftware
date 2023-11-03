using NHA.Website.Software.Entities.Social_Entities;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHA.Website.Software.Services.Social
{
    public interface IPostImageRepository : IGenericRepository<PostImage>
    {
        public Task<List<PostImage>> GetPostImagesAsync(int? postId);
    }
}
