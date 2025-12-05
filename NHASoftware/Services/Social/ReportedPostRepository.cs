using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Social_Entities;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Services.Social;
public class ReportedPostRepository : GenericRepository<ReportedPost>, IReportedPostRepository
{
    public ReportedPostRepository(ApplicationDbContext context) : base(context)
    {

    }
}
