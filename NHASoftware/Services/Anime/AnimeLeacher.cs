using NHA.Website.Software.Entities.Anime;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;

namespace NHA.Website.Software.Services.Anime
{
    public class AnimeLeecher(IUnitOfWork unitOfWork, ILogger<AnimeLeecher> logger) : IAnimeLeecher
    {
        private int _pageNumber = 1;


        public async Task LoadExternalAnime()
        {
            HashSet<string> knownAnimeList = [];
            var currentAnime = await unitOfWork.AnimePageRepository.GetAllAsync();

            foreach (var anime in currentAnime)
            {
                knownAnimeList.Add(anime.AnimeName);
            }
            
            bool hasMore = true;
            using var http = new HttpClient();

            while (hasMore)
            {
                var url = $"https://api.jikan.moe/v4/anime?page={this._pageNumber}&limit=25";
                var response = await http.GetFromJsonAsync<ApiResponse>(url);

                if (response?.data.Count > 0)
                {
                    foreach (var anime in response.data)
                    {
                        if (anime is not { title_english: not null, title: not null, title_japanese: not null }) continue;

                        var streamingUrl = $"https://api.jikan.moe/v4/anime/{anime.mal_id}/streaming";
                        StreamingResponse? streamingResponse = null;

                        try
                        {
                            streamingResponse = await http.GetFromJsonAsync<StreamingResponse>(streamingUrl);
                        }
                        catch (Exception e)
                        {
                            logger.LogError(e.Message);
                        }

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
                                AnimeImageUrl = anime.images.jpg.large_image_url,
                                AnimeStatus = anime.status,
                                AnimeJikanScore = anime.score,
                                AnimeGenres = string.Join(';', anime.genres.Select(x => x.name)),
                                AnimeBackground = anime.background ?? "",
                                TrailerUrl = anime.trailer.embed_url ?? "",
                                EpisodeCount = anime.episodes ?? 1,
                            };

                            if (streamingResponse != null)
                            {
                                animePage.Platforms = string.Join(';', streamingResponse.data.Select(x => x.name));
                            }

                            await unitOfWork.AnimePageRepository.AddAsync(animePage);
                            knownAnimeList.Add(name);
                        }
                        else
                        {
                            //Exists we just want ot handle certain updates
                            var animePages = await unitOfWork.AnimePageRepository.
                                FindAsync(x => x.AnimeName.Equals(anime.title_english) || x.AnimeName.Equals(anime.title_japanese) || x.AnimeName.Equals(anime.title));
                            
                            var summary = anime.synopsis ?? string.Empty;

                            foreach (var animePage in animePages)
                            {
                                animePage.AnimeSummary = summary;
                                animePage.AnimeImageUrl = anime.images.jpg.large_image_url;
                                animePage.AnimeStatus = anime.status;
                                animePage.AnimeJikanScore = anime.score;
                                animePage.AnimeGenres = string.Join(';', anime.genres.Select(x => x.name));
                                animePage.AnimeBackground = anime.background ?? "";
                                animePage.TrailerUrl = anime.trailer.embed_url ?? "";
                                animePage.EpisodeCount = anime.episodes ?? 1;

                                if (streamingResponse != null)
                                {
                                    animePage.Platforms = string.Join(';', streamingResponse.data.Select(x => x.name));
                                }
                            }
                        }

                        await Task.Delay(1500);
                    }

                    var affectedRows = await unitOfWork.CompleteAsync();
                    this._pageNumber++;
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
            public string? background { get; set; } = "";
            public List<Genre> genres { get; set; } = [];
            public ImageGroup images { get; set; } = new ImageGroup();
            public string? title_japanese = "";
            public Trailer trailer { get; set; } = new Trailer();
        }

        public class Trailer
        {
            public string? youtube_id { get; set; }
            public string? url { get; set; }
            public string? embed_url { get; set; }
        }

        public class Genre
        {
            public string name { get; set; } = "";
        }

        public class ImageGroup
        {
            public ImageType jpg { get; set; } = new ImageType();
        }

        public class StreamingResponse
        {
            public List<StreamingService> data { get; set; } = [];
        }

        public class StreamingService
        {
            public string name { get; set; } = "";
            public string url { get; set; } = "";
        }

        public class ImageType
        {
            public string image_url { get; set; } = "";
            public string small_image_url { get; set; } = "";
            public string large_image_url { get; set; } = "";

        }

        public class ApiResponse
        {
            public List<Anime> data { get; set; } = [];
        }
    }
}
