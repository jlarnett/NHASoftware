using NHASoftware.DBContext;
using NHASoftware.Entities.Social_Entities;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHA.Website.Software.Services.Social
{
    public class UserLikeRepository : GenericRepository<UserLikes>, IUserLikeRepository
    {
        public UserLikeRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
