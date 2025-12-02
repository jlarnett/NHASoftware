using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Social_Entities;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
namespace NHA.Website.Software.Services.Social;
public class HiddenPostRepository : GenericRepository<HiddenPost>, IHiddenPostRepository
{
    public HiddenPostRepository(ApplicationDbContext context) : base(context)
    {

    }
}
