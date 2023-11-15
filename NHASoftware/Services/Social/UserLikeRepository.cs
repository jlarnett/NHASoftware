using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Social_Entities;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Services.Social;
public class UserLikeRepository : GenericRepository<UserLikes>, IUserLikeRepository
{
    public UserLikeRepository(ApplicationDbContext context) : base(context)
    {

    }
}
