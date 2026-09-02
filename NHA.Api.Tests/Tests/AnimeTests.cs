using NHA.Api.Tests.Setup;
using NHA.Website.Software.Entities.Anime;
using Refit;

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

            Assert.True(response.IsSuccessStatusCode, $"Expected a success status code from {ApiBaseUrl}/api/AnimePages?pageNumber=1 but received {(int?)response.StatusCode}.");
            Assert.NotNull(response.Content);
            Assert.Equal(50, response.Content.Count);
        }
    }
}
