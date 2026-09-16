using NHA.Api.Tests.Setup;
using NHA.Website.Software.Entities.Anime;
using Refit;
using System.Net;
using NHA.Website.Software.Controllers.WebAPIs.Anime;

namespace NHA.Api.Tests.Tests
{
    public class AnimeTests : BaseTestEnvironmentFixture
    {
        [Fact]
        public async Task GetAnimePages_ReturnsSuccessfulResponse_WhenAppIsRunning()
        {
            ApiResponse<List<AnimePage>> response;

            try
            {
                response = await AnimeApi.GetAnimePagesAsync(1);
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException(
                    $"Start NHA.Website.Software before running this test. Configure NHA_API_BASE_URL if it is not running at '{ApiBaseUrl}'.",
                    ex);
            }

            Console.WriteLine("testing ruleset");
            Assert.True(response.IsSuccessStatusCode,
                $"Expected a success status code from {ApiBaseUrl}/api/AnimePages?pageNumber=1 but received {(int?)response.StatusCode}.");
            Assert.NotNull(response.Content);
        }

        [Fact]
        public async Task CreateAnimePageTest()
        {
            // Arrange
            var authenticatedUser = await CreateAuthenticatedUserAsync();

            var anime = new CreateAnimePageRequest()
            {
                AnimeName = Faker.Vehicle.Model(),
                AnimeEnglishName = Faker.Vehicle.Manufacturer(),
                AnimeJapaneseName = Faker.Name.JobDescriptor(),
                AnimeGenres = "Action",
                AnimeBackground = Faker.Lorem.Paragraphs(),
                AnimeSummary = Faker.Lorem.Paragraphs(),
                AnimeImageUrl = Faker.Image.PlaceImgUrl(),
                AnimeStatus = "Finished Airing",
                Platforms = "Crunchyroll;Netflix;Shahid",
                UpVotes = 10,
                DownVotes = 20,
                TrailerUrl = Faker.Internet.Url(),
                AnimeJikanScore = 5.5,
            };

            // Act
            ApiResponse<AnimePage> response;

            try
            {
                response = await authenticatedUser.AnimeApi.CreateAnimePageAsync(anime);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            if (response.Content != null)
            {
                Assert.Equal(anime.AnimeName, response.Content.AnimeName);
                Assert.Equal(anime.AnimeEnglishName, response.Content.AnimeEnglishName);
                Assert.Equal(anime.AnimeJapaneseName, response.Content.AnimeJapaneseName);
                Assert.Equal(anime.AnimeGenres, response.Content.AnimeGenres);
                Assert.Equal(anime.AnimeBackground, response.Content.AnimeBackground);
                Assert.Equal(anime.AnimeSummary, response.Content.AnimeSummary);
                Assert.Equal(0, response.Content.UpVotes);
                Assert.Equal(0, response.Content.DownVotes);
                Assert.Equal(anime.TrailerUrl, response.Content.TrailerUrl);
                Assert.Equal(anime.AnimeStatus, response.Content.AnimeStatus);
                Assert.Equal(anime.AnimeImageUrl, response.Content.AnimeImageUrl);
                Assert.Equal(anime.AnimeJikanScore, response.Content.AnimeJikanScore);
                Assert.Equal(anime.AnimeImageUrl, response.Content.AnimeImageUrl);

            }
        }

        [Fact]
        public async Task CreateAnimePageTestUnauthorized()
        {
            // Arrange
            var anonymousUser = CreateAnonymousSession();

            var anime = new CreateAnimePageRequest()
            {
                AnimeName = Faker.Vehicle.Model(),
                AnimeEnglishName = Faker.Vehicle.Manufacturer(),
                AnimeJapaneseName = Faker.Name.JobDescriptor(),
                AnimeGenres = "Action",
                AnimeBackground = Faker.Lorem.Paragraphs(),
                AnimeSummary = Faker.Lorem.Paragraphs(),
                AnimeImageUrl = Faker.Image.PlaceImgUrl(),
                AnimeStatus = "Finished Airing",
                Platforms = "Crunchyroll;Netflix;Shahid",
                UpVotes = 10,
                DownVotes = 20,
                TrailerUrl = Faker.Internet.Url(),
                AnimeJikanScore = 5.5,
            };

            // Act
            ApiResponse<AnimePage> response;

            try
            {
                response = await anonymousUser.AnimeApi.CreateAnimePageAsync(anime);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
