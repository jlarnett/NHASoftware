using NHA.Website.Software.Entities.FriendSystem;
using NHASoftware.Entities.Social_Entities;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHASoftware.Services.FriendSystem
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
