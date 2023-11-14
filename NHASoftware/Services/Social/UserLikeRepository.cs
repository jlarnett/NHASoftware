using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHASoftware.DBContext;
using NHASoftware.Entities.Social_Entities;

namespace NHA.Website.Software.Services.Social
{
    public class UserLikeRepository : GenericRepository<UserLikes>, IUserLikeRepository
    {
        public UserLikeRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
