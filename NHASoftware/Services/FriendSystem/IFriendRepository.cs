using NHA.Website.Software.Entities.FriendSystem;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;

namespace NHA.Website.Software.Services.FriendSystem
{
    public interface IFriendRepository : IGenericRepository<Friends>
    {
        /// <summary>
        /// Gets Application Users Friend List ASYNC
        /// </summary>
        /// <param name="userId">ApplicationUsers id you want to retrieve friends for</param>
        /// <returns>List of friend records associated to User Id. </returns>
        public Task<List<Friends>> GetUsersFriendListAsync(string userId);
    }
}
