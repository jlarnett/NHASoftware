using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Sponsors;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;

namespace NHA.Website.Software.Services.Sponsors
{
    public class SponsorAdRepository : GenericRepository<SponsorAd>, ISponsorAdRepository
    {
        public SponsorAdRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
