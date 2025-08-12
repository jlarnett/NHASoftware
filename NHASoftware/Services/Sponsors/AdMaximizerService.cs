using NHA.Website.Software.Entities.Anime;
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

        public async Task PickFeaturedAnime()
        {
            var popularAnime = await _unitOfWork.AnimePageRepository.FindAsync(a => a.AnimeJikanScore >= 3.8 && !a.Featured);
            var currentlyFeaturedAnime = await _unitOfWork.AnimePageRepository.FindAsync(ap => ap.Featured);

            foreach (var page in currentlyFeaturedAnime)
            {
                page.Featured = false;
            }

            var animePages = popularAnime as AnimePage[] ?? popularAnime.ToArray();
            var count = animePages.Count();
            var featuredAnime = animePages.ElementAt(Random.Shared.Next(0, count));
            featuredAnime.Featured = true;

            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<SponsorAd>> GetBestAdsForUserAsync()
        {
            int numberOfAdsToReturn = 1;
            var ads = await _unitOfWork.SponsorAdRepository.GetAllAsync();
            var bestAdsForUserAsync = ads as SponsorAd[] ?? ads.ToArray();
            var count = bestAdsForUserAsync.Count();

            //Get 3 random adds
            if (count < numberOfAdsToReturn)
                return bestAdsForUserAsync;

            List<SponsorAd> bestAds = [];
            
            for (int i = 0; i < numberOfAdsToReturn; i++)
            {
                var randomIndex = new Random((int)DateTime.UtcNow.Ticks).Next(count);
                bestAds.Add(bestAdsForUserAsync[randomIndex]);
            }

            foreach (var ad in bestAds)
            {
                ad.Views++;
            }

            await _unitOfWork.CompleteAsync();
            return bestAds;
        }
    }
}
