using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using NHASoftware.DBContext;
using NHASoftware.Entities.Forums;

namespace NHA.Website.Software.Services.Forums
{
    public class ForumSectionRepository : GenericRepository<ForumSection>, IForumSectionRepository
    {
        public ForumSectionRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
