using Bogus;
using NHA.Website.Software.ConsumableEntities.DTOs;
using System.Net;
using NHA.Api.Tests.Setup;

namespace NHA.Api.Tests.Tests
{
    public class SocialTests: BaseTestEnvironmentFixture
    {
        [Fact]
        public async Task CreateValidPost()
        {
            // Arrange
            var authenticatedUser = await CreateAuthenticatedUserAsync();

            var post = new PostDTO
            {
                Summary = Faker.Lorem.Paragraph(),
            };

            // Act
            var response = await authenticatedUser.PostFormWithAntiforgeryAsync(
                "/api/posts/BasicPost",
                [new KeyValuePair<string, string>(nameof(PostDTO.Summary), post.Summary)]);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task CreatePostNotAuthenticated()
        {
            // Arrange
            await using var anonUser = CreateAnonymousSession(allowAutoRedirect: false);

            var post = new PostDTO
            {
                Summary = Faker.Lorem.Paragraph(),
            };

            // Act
            var response = await anonUser.PostFormWithAntiforgeryAsync(
                "/api/posts/BasicPost",
                [new KeyValuePair<string, string>(nameof(PostDTO.Summary), post.Summary)]);

            // Assert
            Assert.Equal(HttpStatusCode.Found, response.StatusCode);
            Assert.NotNull(response.Headers.Location);
        }

    }
}
