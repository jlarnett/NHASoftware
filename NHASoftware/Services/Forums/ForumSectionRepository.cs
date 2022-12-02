using NHASoftware.DBContext;
using NHASoftware.Entities.Forums;
using NHASoftware.Services.RepositoryPatternFoundationals;

namespace NHASoftware.Services.Forums
{
    public class ForumSectionRepository : GenericRepository<ForumSection>, IForumSectionRepository
    {
        public ForumSectionRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
