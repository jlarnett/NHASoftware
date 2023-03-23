using NHASoftware.Entities.Social_Entities;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHASoftware.Services.Social
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        public Task<List<Post>> GetAllPostsWithIncludesAsync();
        public Task<List<Post>> GetUsersSocialPostsAsync(string userId);
        public void AddUsingSproc(Post post);
    }
}
