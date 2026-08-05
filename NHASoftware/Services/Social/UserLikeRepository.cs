using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Social_Entities;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Services.Social;
public class UserLikeRepository : GenericRepository<UserLikes>, IUserLikeRepository
{
    public UserLikeRepository(ApplicationDbContext context) : base(context)
    {

    }

    public async Task<List<UserLikes>> GetByPostIdsAsync(IEnumerable<int> postIds)
    {
        var postIdsList = postIds.Distinct().ToList();

        if (postIdsList.Count == 0)
        {
            return [];
        }

        return await _context.Set<UserLikes>()
            .AsNoTracking()
            .Where(userLike => postIdsList.Contains(userLike.PostId))
            .ToListAsync();
    }
}
