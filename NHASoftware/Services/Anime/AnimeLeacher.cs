using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Anime;

namespace NHA.Website.Software.Services.Anime
{
    public class AnimeLeecher : IAnimeLeecher
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AnimeLeecher> _logger;

        public AnimeLeecher(ApplicationDbContext context, ILogger<AnimeLeecher> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LoadExternalAnime()
        {
            var animeList = await FetchAllAnimeAsync();
            
            HashSet<string> knownAnimeList = [];
            var currentAnime = await _context.AnimePages!.ToListAsync();


            foreach (var anime in currentAnime)
            {
                knownAnimeList.Add(anime.AnimeName);
            }

            foreach (var anime in animeList.Where(anime => !knownAnimeList.Contains(anime.title_english)))
            {
                await _context.AnimePages!.AddAsync(new AnimePage()
                {
                    AnimeName = anime.title_english,
                    AnimeSummary = anime.synopsis,
                });
            }

            await _context.SaveChangesAsync();
        }
        
        public class Anime
        {
            public int mal_id { get; set; }
            public string title { get; set; } = "";
            public string title_english { get; set; } = "";
            public string status { get; set; } = "";
            public int? episodes { get; set; }
            public double? score { get; set; }
            public string synopsis { get; set; } = "";
            public List<Genre> genres { get; set; } = [];
            public ImageGroup images { get; set; } = new ImageGroup();
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

        public async Task<List<Anime>> FetchAllAnimeAsync()
        {
            var allAnime = new List<Anime>();
            int page = 1;
            bool hasMore = true;

            using var http = new HttpClient();

            while (hasMore)
            {
                var url = $"https://api.jikan.moe/v4/anime?page={page}&limit=25";
                var response = await http.GetFromJsonAsync<ApiResponse>(url);

                if (response?.data?.Count > 0)
                {
                    allAnime.AddRange(response.data);
                    page++;
                }
                else
                {
                    hasMore = false;
                }

                // Optional: sleep to respect rate limits (Jikan recommends)
                await Task.Delay(500);
            }

            return allAnime;
        }
    }
    
    
}
