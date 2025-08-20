using NHA.Website.Software.Entities.Sponsors;
using System.Threading.Tasks;

namespace NHA.Website.Software.Services.Sponsors
{
    public interface IAdMaximizerService
    {
        Task<IEnumerable<SponsorAd>> GetBestAdsForUserAsync();
        Task PickFeaturedAnime();
        Task PickFeaturedGame();
    }
}
