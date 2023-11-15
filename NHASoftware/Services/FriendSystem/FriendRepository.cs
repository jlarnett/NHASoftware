using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.FriendSystem;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Services.FriendSystem;
public class FriendRepository : GenericRepository<Friends>, IFriendRepository
{
    public FriendRepository(ApplicationDbContext context) : base(context)
    {

    }

    /// <summary>
    /// Returns all friend records associated with userId
    /// </summary>
    /// <param name="userId">ApplicationUsers id that you want to retrieve friend records for</param>
    /// <returns>List of friend records associated to user. Includes ApplicationUser data</returns>
    public async Task<List<Friends>> GetUsersFriendListAsync(string userId)
    {
        return await _context.Friends!.Include(f => f.FriendOne).Include(f => f.FriendTwo).Where(f => f.FriendOneId.Equals(userId) || f.FriendTwoId.Equals(userId)).ToListAsync();
    }
}
