using NHA.Api.Tests.Setup;

namespace NHA.Api.Tests.Tests
{
    public class SearchTests: BaseTestEnvironmentFixture
    {
        [Fact]
        public async Task ValidAnimeSearchReturnsResults()
        {
            // Arrange
            var searchTerm = "Naruto";

            // Act
            var response = await SearchApi.GetSearchAsync(searchTerm);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);
        }

        [Fact]
        public async Task ValidUserSearchReturnsResults()
        {
            // Arrange
            var searchTerm = "im10g@hotmail.com";

            // Act
            var response = await SearchApi.GetSearchAsync(searchTerm);

            // Assert
            Assert.True(response.IsSuccessStatusCode);
            Assert.NotNull(response.Content);
        }
    }
}
