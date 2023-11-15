using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Forums;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Services.Forums;
public class ForumSectionRepository : GenericRepository<ForumSection>, IForumSectionRepository
{
    public ForumSectionRepository(ApplicationDbContext context) : base(context)
    {
    }
}
