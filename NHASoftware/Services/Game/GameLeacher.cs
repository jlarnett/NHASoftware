using DnsClient;
using Microsoft.Extensions.Logging;
using NHA.Website.Software.Entities.Game;
using NHA.Website.Software.Services.RepositoryPatternFoundationals;
using System.Net.Http.Json;

namespace NHA.Website.Software.Services.Game
{
    public class GameLeecher : IGameLeecher
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<GameLeecher> _logger;
        private readonly IConfiguration _configuration;
        private int pageNumber = 1;
        

        public GameLeecher(IUnitOfWork unitOfWork, ILogger<GameLeecher> logger, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task LoadExternalGameInformation()
        {
            HashSet<string> knownGames = [];
            var currentGames = await _unitOfWork.GamePageRepository.GetAllAsync();

            foreach (var game in currentGames)
                knownGames.Add(game.Name);

            bool hasMore = true;
            using var http = new HttpClient();

            while (hasMore)
            {
                try
                {
                    var url = $"https://api.rawg.io/api/games?page={this.pageNumber}&page_size=40&key={_configuration["Rawg:ApiKey"]}";
                    var response = await http.GetFromJsonAsync<RawgApiResponse>(url);

                    if (response?.results.Count > 0)
                    {
                        foreach (var game in response.results)
                        {
                            if (string.IsNullOrWhiteSpace(game.name)) continue;

                            var rawgGameDetailUrl = $"https://api.rawg.io/api/games/{game.slug}?key={_configuration["Rawg:ApiKey"]}";
                            var gameDetailResponse = await http.GetFromJsonAsync<GameDetail>(rawgGameDetailUrl);

                            var exists = knownGames.Contains(game.name);

                            if (!exists)
                            {

                                var newGame = new GamePage
                                {
                                    Name = game.name,
                                    ImageUrl = game.background_image ?? "",
                                    Genres = string.Join(';', game.genres.Select(x => x.name)),
                                    GameScore = game.rating
                                };

                                if (gameDetailResponse is not null)
                                {
                                    newGame.Summary = gameDetailResponse.description ?? "";
                                    newGame.Released = gameDetailResponse.released ?? "";
                                    newGame.Platforms = string.Join(';', game.platforms.Select(x => x.platform.name));
                                }


                                _unitOfWork.GamePageRepository.Add(newGame);
                                knownGames.Add(game.name);
                            }
                            else
                            {
                                var gamePages = await _unitOfWork.GamePageRepository.FindAsync(
                                    x => x.Name.Equals(game.name)
                                );

                                foreach (var gamePage in gamePages)
                                {
                                    if (gameDetailResponse is not null)
                                        gamePage.Summary = gameDetailResponse.description ?? "";
                                    gamePage.ImageUrl = game.background_image ?? "";
                                    gamePage.Genres = string.Join(';', game.genres.Select(x => x.name));
                                    gamePage.GameScore = game.rating;

                                    if (gameDetailResponse is not null)
                                    {
                                        gamePage.Summary = gameDetailResponse.description ?? "";
                                        gamePage.Released = gameDetailResponse.released ?? "";
                                        gamePage.Platforms = string.Join(';', game.platforms.Select(x => x.platform.name));
                                    }
                                }

                            }
                        }

                        var results = await _unitOfWork.CompleteAsync();
                        pageNumber++;
                    }
                    else
                    {
                        hasMore = false;
                    }

                    await Task.Delay(2500); // Respect rate limits
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }

            }
        }

        public class RawgApiResponse
        {
            public List<RawgGame> results { get; set; } = new();
        }

        public class RawgGame
        {
            public int id { get; set; }
            public string slug { get; set; } = string.Empty;
            public string name { get; set; } = string.Empty;
            public string? released { get; set; }
            public bool tba { get; set; }
            public string? background_image { get; set; }
            public double rating { get; set; }
            public int rating_top { get; set; }
            public List<Rating> ratings { get; set; } = new();
            public int ratings_count { get; set; }
            public int? reviews_text_count { get; set; }
            public int added { get; set; }
            public Dictionary<string, object> added_by_status { get; set; } = new();
            public int? metacritic { get; set; }
            public int playtime { get; set; }
            public int suggestions_count { get; set; }
            public DateTime? updated { get; set; }
            public EsrbRating? esrb_rating { get; set; }
            public List<RawgPlatform> platforms { get; set; } = new();
            public List<RawgGenre> genres { get; set; } = new();
        }
        
        public class RawgPlatform
        {
            public PlatformInfo platform { get; set; } = new();
            public string? released_at { get; set; }
            public Requirements? requirements { get; set; }
        }

        public class RawgGenre
        {
            public string name { get; set; } = "";
        }
        
        public class GameDetail
        {
            public int id { get; set; }
            public string slug { get; set; } = string.Empty;
            public string name { get; set; } = string.Empty;
            public string name_original { get; set; } = string.Empty;
            public string description { get; set; } = string.Empty;
            public int? metacritic { get; set; }
            public List<MetacriticPlatform> metacritic_platforms { get; set; } = new();
            public string? released { get; set; }
            public bool tba { get; set; }
            public DateTime? updated { get; set; }
            public string? background_image { get; set; }
            public string? background_image_additional { get; set; }
            public string? website { get; set; }
            public double rating { get; set; }
            public int rating_top { get; set; }
            public List<Rating> ratings { get; set; } = [];
            public Dictionary<string, object> reactions { get; set; } = new();
            public int added { get; set; }
            public Dictionary<string, object> added_by_status { get; set; } = new();
            public int playtime { get; set; }
            public int screenshots_count { get; set; }
            public int movies_count { get; set; }
            public int creators_count { get; set; }
            public int achievements_count { get; set; }
            public int? parent_achievements_count { get; set; }
            public string? reddit_url { get; set; }
            public string? reddit_name { get; set; }
            public string? reddit_description { get; set; }
            public string? reddit_logo { get; set; }
            public int? reddit_count { get; set; }
            public int? twitch_count { get; set; }
            public int? youtube_count { get; set; }
            public int? reviews_text_count { get; set; }
            public int? ratings_count { get; set; }
            public int suggestions_count { get; set; }
            public List<string> alternative_names { get; set; } = new();
            public string? metacritic_url { get; set; }
            public int? parents_count { get; set; }
            public int? additions_count { get; set; }
            public int? game_series_count { get; set; }
            public EsrbRating? esrb_rating { get; set; }
            public List<GamePlatform> platforms { get; set; } = new();
        }

        public class MetacriticPlatform
        {
            public int metascore { get; set; }
            public string url { get; set; } = string.Empty;
        }

        public class EsrbRating
        {
            public int id { get; set; }
            public string slug { get; set; } = string.Empty;
            public string name { get; set; } = string.Empty;
        }

        public class GamePlatform
        {
            public PlatformInfo platform { get; set; } = new();
            public string? released_at { get; set; }
            public Requirements? requirements { get; set; }
        }

        public class PlatformInfo
        {
            public int id { get; set; }
            public string slug { get; set; } = string.Empty;
            public string name { get; set; } = string.Empty;
        }

        public class Requirements
        {
            public string? minimum { get; set; }
            public string? recommended { get; set; }
        }
        
        public class Rating
        {
            public int id { get; set; }
            public string title { get; set; } = string.Empty;
            public int count { get; set; }
            public double percent { get; set; }
        }

    }
}
