using System.Net;
using NHA.Api.Tests.Endpoints;
using NHA.Api.Tests.Setup;

namespace NHA.Api.Tests.Tests
{
    public class UserTests: BaseTestEnvironmentFixture
    {
        [Fact]
        public async Task CreateNewUser()
        {
            // Arrange
            const string password = "Aa1!Aa1!";

            var newUser = new RegisterUserRequest()
            {
                Email = Faker.Internet.Email(),
                Password = password,
                ConfirmPassword = password
            };

            // Act
            var response = await UsersApi.RegisterAsync(newUser);

            // Assert
            Assert.True(response.StatusCode == HttpStatusCode.Created);
            Assert.NotNull(response.Content);
            Assert.Equal("Anonymous Gangsta", response.Content!.DisplayName);
        }

        [Fact]
        public async Task DeleteCurrentUser()
        {
            const string password = "Aa1!Aa1!";

            var newUser = new RegisterUserRequest()
            {
                Email = Faker.Internet.Email(),
                Password = password,
                ConfirmPassword = password
            };

            var registerResponse = await UsersApi.RegisterAsync(newUser);

            Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

            var deleteResponse = await UsersApi.DeleteAsync(new DeleteUserRequest
            {
                Password = password
            });

            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
            Assert.NotNull(deleteResponse.Content);
            Assert.Equal(registerResponse.Content!.UserId, deleteResponse.Content!.UserId);
        }

    }
}
