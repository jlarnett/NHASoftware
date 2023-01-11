using NHASoftware.DBContext;
using NHASoftware.Entities.FriendSystem;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHASoftware.Services.FriendSystem
{
    public class FriendRepository : GenericRepository<Friends>, IFriendRepository
    {
        public FriendRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
