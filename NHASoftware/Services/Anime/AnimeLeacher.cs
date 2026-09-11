using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using NHA.Website.Software.Entities.Anime;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;

namespace NHA.Website.Software.Services.Anime
{
    public class AnimeLeecher(IUnitOfWork unitOfWork, ILogger<AnimeLeecher> logger) : IAnimeLeecher
    {
        private const string baseUrl = "https://api.tenrai.org/v1/anime";
        private int _pageNumber = 1;


        public async Task LoadExternalAnime()
        {
            var currentAnime = (await unitOfWork.AnimePageRepository.GetAllAsync()).ToList();
            var currentAnimeEpisodes = (await unitOfWork.AnimeEpisodeRepository.GetAllAsync()).ToList();
            
            bool hasMore = true;
            using var http = new HttpClient();

            while (hasMore)
            {
                var url = $"{baseUrl}?page={this._pageNumber}&limit=25";
                ApiResponse? response;

                try
                {
                    response = await http.GetFromJsonAsync<ApiResponse>(url);
                }
                catch (Exception e)
                {
                    logger.LogError(e, e.Message);
                    continue;
                }


                if (response?.data.Count > 0)
                {
                    foreach (var anime in response.data)
                    {
                        var animeNames = GetAnimeNameVariants(anime);

                        if (animeNames.Count == 0)
                            continue;

                        var displayName = BuildAnimeName(anime);
                        var englishName = CleanAnimeName(anime.title_english);
                        var japaneseName = CleanAnimeName(anime.title_japanese);

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

                        var matchingAnimePages = currentAnime
                            .Where(x => DoesAnimePageMatch(x, animeNames))
                            .ToList();

                        var exists = matchingAnimePages.Count != 0;
                            
                        if (!exists)
                        {
                            var summary = anime.synopsis ?? string.Empty;
                            
                            var animePage = new AnimePage()
                            {
                                AnimeName = displayName,
                                AnimeEnglishName = englishName,
                                AnimeJapaneseName = japaneseName,
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
                            currentAnime.Add(animePage);
                        }
                        else
                        {
                            var episodeUrl = $"https://api.jikan.moe/v4/anime/{anime.mal_id}/episodes";
                            EpisodeResponse? episodeResponse = null;

                            try
                            {
                                episodeResponse = await http.GetFromJsonAsync<EpisodeResponse>(episodeUrl);
                            }
                            catch (Exception e)
                            {
                                logger.LogError(e.Message);
                            }

                            //Exists we just want ot handle certain updates
                            var summary = anime.synopsis ?? string.Empty;

                            foreach (var animePage in matchingAnimePages)
                            {
                                animePage.AnimeName = displayName;
                                animePage.AnimeEnglishName = englishName;
                                animePage.AnimeJapaneseName = japaneseName;
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

                                if (episodeResponse != null)
                                {
                                    foreach (var episode in episodeResponse.data)
                                    {
                                        //If we already have a matching anime episode break
                                        if (currentAnimeEpisodes.Any(x => x.AnimePageId.Equals(animePage.Id) && x.EpisodeName.Equals(episode.Title) &&
                                                                            x.EpisodeNumber.Equals(episode.mal_id)))
                                            continue;

                                        await unitOfWork.AnimeEpisodeRepository.AddAsync(new AnimeEpisode()
                                        {
                                            AnimePageId = animePage.Id,
                                            EpisodeName = episode.Title,
                                            EpisodeSummary = "",
                                            EpisodeNumber = episode.mal_id,
                                            UpVotes = 0,
                                            DownVotes = 0,
                                            EpisodeContainsFiller = episode.Filler,
                                        });
                                    }
                                }
                            }
                        }

                        await Task.Delay(2000);
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
            public string? title_japanese { get; set; } = "";
            public Trailer trailer { get; set; } = new Trailer();
        }

        private static List<string> GetAnimeNameVariants(Anime anime)
        {
            HashSet<string> names = new(StringComparer.OrdinalIgnoreCase);

            AddAnimeName(names, anime.title_english);
            AddAnimeName(names, anime.title);
            AddAnimeName(names, anime.title_japanese);
            AddAnimeName(names, BuildAnimeName(anime));

            return [.. names];
        }

        private static bool DoesAnimePageMatch(AnimePage animePage, IEnumerable<string> animeNames)
        {
            var existingNames = new[]
            {
                animePage.AnimeName,
                animePage.AnimeEnglishName,
                animePage.AnimeJapaneseName,
            };

            return existingNames
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Select(name => name!.Trim())
                .Any(existingName => animeNames.Any(name => existingName.Equals(name, StringComparison.OrdinalIgnoreCase)));
        }

        private static string BuildAnimeName(Anime anime)
        {
            return FirstAvailableName(anime.title_english, anime.title, anime.title_japanese);
        }

        private static string FirstAvailableName(params string?[] names)
        {
            return names.FirstOrDefault(name => !string.IsNullOrWhiteSpace(name))?.Trim() ?? string.Empty;
        }

        private static void AddAnimeName(HashSet<string> names, string? value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                names.Add(value.Trim());
            }
        }

        private static string CleanAnimeName(string? value)
        {
            return value?.Trim() ?? string.Empty;
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

        public class EpisodeResponse
        {
            public List<Episode> data { get; set; } = [];
        }

        public class Episode
        {
            public int mal_id { get; set; }
            public string Title { get; set; } = "";
            public string TitleJapanese { get; set; } = "";
            public string TitleRomanji { get; set; } = "";
            public DateTime Aired { get; set; }
            public double Score { get; set; }
            public bool Filler { get; set; }
            public bool Recap { get; set; }
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
