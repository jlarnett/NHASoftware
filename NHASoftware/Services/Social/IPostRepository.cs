using NHASoftware.Entities.Social_Entities;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHASoftware.Services.Social
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        public Task<List<Post>> GetAllPostsWithIncludesAsync();
        public void AddUsingSproc(Post post);
    }
}
