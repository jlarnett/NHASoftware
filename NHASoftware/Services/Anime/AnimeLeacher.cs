using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Anime;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;

namespace NHA.Website.Software.Services.Anime
{
    public class AnimeLeecher : IAnimeLeecher
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AnimeLeecher> _logger;

        private int pageNumber = 1;

        public AnimeLeecher(IUnitOfWork unitOfWork, ILogger<AnimeLeecher> logger)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }


        public async Task LoadExternalAnime()
        {
            HashSet<string> knownAnimeList = [];
            var currentAnime = await _unitOfWork.AnimePageRepository.GetAllAsync();

            foreach (var anime in currentAnime)
            {
                knownAnimeList.Add(anime.AnimeName);
            }
            
            bool hasMore = true;
            using var http = new HttpClient();

            while (hasMore)
            {
                var url = $"https://api.jikan.moe/v4/anime?page={this.pageNumber}&limit=25";
                var response = await http.GetFromJsonAsync<ApiResponse>(url);

                if (response?.data.Count > 0)
                {
                    foreach (var anime in response.data)
                    {
                        if (anime is not { title_english: not null, title: not null, title_japanese: not null }) continue;
                        
                        var exists = knownAnimeList.Contains(anime.title_english) ||
                                     knownAnimeList.Contains(anime.title) || knownAnimeList.Contains(anime.title_japanese);
                            
                        if (!exists)
                        {
                            if(anime.title_english == null && anime.title == null) 
                                continue;

                            var name = (string.IsNullOrEmpty(anime.title_english)
                                ? anime.title_english
                                : anime.title) ?? string.Empty;

                            if (name.Equals(""))
                                name = anime.title_japanese ?? string.Empty;
                            
                            var summary = anime.synopsis ?? string.Empty;
                            
                            var animePage = new AnimePage()
                            {
                                AnimeName = name,
                                AnimeSummary = summary,
                                AnimeImageUrl = anime.images.jpg.image_url,
                                AnimeStatus = anime.status,
                                AnimeJikanScore = anime.score,
                                AnimeGenres = string.Join(';', anime.genres.Select(x => x.name)),
                            };

                            _unitOfWork.AnimePageRepository.Add(animePage);
                            knownAnimeList.Add(name);
                        }
                        else
                        {
                            //Exists we just want ot handle certain updates
                            var animePages = await _unitOfWork.AnimePageRepository.
                                FindAsync(x => x.AnimeName.Equals(anime.title_english) || x.AnimeName.Equals(anime.title_japanese) || x.AnimeName.Equals(anime.title));
                            
                            var summary = anime.synopsis ?? string.Empty;

                            foreach (var animePage in animePages)
                            {
                                animePage.AnimeSummary = summary;
                                animePage.AnimeImageUrl = anime.images.jpg.image_url;
                                animePage.AnimeStatus = anime.status;
                                animePage.AnimeJikanScore = anime.score;
                                animePage.AnimeGenres = string.Join(';', anime.genres.Select(x => x.name));
                            }
                        }
                    }

                    var affectedRows = await _unitOfWork.CompleteAsync();
                    this.pageNumber++;
                }
                else
                {
                    hasMore = false;
                }

                // Optional: sleep to respect rate limits (Jikan recommends)
                await Task.Delay(2500);
            }
        }
        
        public class Anime
        {
            public int mal_id { get; set; }
            public string? title { get; set; } = "";
            public string? title_english { get; set; } = "";
            public string status { get; set; } = "";
            public int? episodes { get; set; }
            public double? score { get; set; }
            public string? synopsis { get; set; } = "";
            public List<Genre> genres { get; set; } = [];
            public ImageGroup images { get; set; } = new ImageGroup();
            public string? title_japanese = "";
        }

        public class Genre
        {
            public string name { get; set; } = "";
        }

        public class ImageGroup
        {
            public ImageType jpg { get; set; } = new ImageType();
        }

        public class ImageType
        {
            public string image_url { get; set; } = "";
        }

        public class ApiResponse
        {
            public List<Anime> data { get; set; } = [];
        }
    }
}
