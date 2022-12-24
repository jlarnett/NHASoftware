using NHASoftware.DBContext;
using NHASoftware.Entities.Social_Entities;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHASoftware.Services.Social
{
    public class UserLikeRepository : GenericRepository<UserLikes>, IUserLikeRepository
    {
        public UserLikeRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}
