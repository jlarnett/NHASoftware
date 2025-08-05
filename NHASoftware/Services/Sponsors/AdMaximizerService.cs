using NHA.Website.Software.Entities.Sponsors;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;

namespace NHA.Website.Software.Services.Sponsors
{
    public class AdMaximizerService: IAdMaximizerService
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public AdMaximizerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SponsorAd>> GetBestAdsForUserAsync()
        {
            var ads = await _unitOfWork.SponsorAdRepository.GetAllAsync();
            var bestAdsForUserAsync = ads as SponsorAd[] ?? ads.ToArray();
            var count = bestAdsForUserAsync.Count();

            //Get 3 random adds
            if (count < 3)
                return bestAdsForUserAsync;

            List<SponsorAd> bestAds = [];
            
            for (int i = 0; i < 3; i++)
            {
                var randomIndex = new Random((int)DateTime.UtcNow.Ticks).Next(count);
                bestAds.Add(bestAdsForUserAsync[randomIndex]);
            }

            return bestAds;
        }
    }
}
