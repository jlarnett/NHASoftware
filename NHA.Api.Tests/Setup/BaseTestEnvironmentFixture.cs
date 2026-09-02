using Bogus;
using NHA.Api.Tests.Endpoints;

namespace NHA.Api.Tests.Setup
{
    /// <summary>
    /// Primary test environment fixture. Sets up the base url and authentication for API Tests
    /// </summary>
    public class BaseTestEnvironmentFixture : IAsyncLifetime
    {
        /// <summary>
        /// Tracks authenticated user sessions created during a test so they can be cleaned up automatically.
        /// </summary>
        private readonly List<AuthenticatedUserSession> _authenticatedUsers = [];

        /// <summary>
        /// Exposes the configured API base URL to derived tests.
        /// </summary>
        private protected static string ApiBaseUrl => ApiTestEnvironment.ApiBaseUrl;

        /// <summary>
        /// Reuses a shared set of API clients for test scenarios that do not need isolated cookies.
        /// </summary>
        private static readonly ApiClients SharedApiClients = ApiTestEnvironment.CreateApiClients(ApiTestEnvironment.SharedHttpClient);

        /// <summary>
        /// Initializes the shared Refit clients exposed to derived test classes.
        /// </summary>
        private protected BaseTestEnvironmentFixture()
        {
            AnimeApi = SharedApiClients.AnimeApi;
            SearchApi = SharedApiClients.SearchApi;
            SocialApi = SharedApiClients.SocialApi;
            UsersApi = SharedApiClients.UsersApi;
        }

        /// <summary>
        /// Provides access to anime API endpoints for derived tests.
        /// </summary>
        private protected IAnimeApi AnimeApi { get; }

        /// <summary>
        /// Provides access to search API endpoints for derived tests.
        /// </summary>
        private protected ISearchApi SearchApi { get; }

        /// <summary>
        /// Provides access to social API endpoints for derived tests.
        /// </summary>
        private protected ISocialApi SocialApi { get; }

        /// <summary>
        /// Provides access to user API endpoints for derived tests.
        /// </summary>
        private protected IUsersApi UsersApi { get; }

        /// <summary>
        /// Supplies randomized test data generation helpers.
        /// </summary>
        private protected Faker Faker { get; } = new();

        /// <summary>
        /// Performs fixture startup work before tests run.
        /// </summary>
        public ValueTask InitializeAsync()
        {
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// Disposes any authenticated sessions created by the current test instance.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            foreach (var authenticatedUser in _authenticatedUsers)
            {
                await authenticatedUser.DisposeAsync();
            }

            _authenticatedUsers.Clear();
        }

        /// <summary>
        /// Creates a new anonymous session with its own cookie container.
        /// </summary>
        /// <param name="allowAutoRedirect"><c>true</c> to follow redirects automatically; otherwise, <c>false</c>.</param>
        private protected AnonymousApiSession CreateAnonymousSession(bool allowAutoRedirect = true) => new(allowAutoRedirect);

        /// <summary>
        /// Creates an authenticated test user session and registers it for cleanup.
        /// </summary>
        /// <param name="displayName">Optional display name to assign to the generated user.</param>
        /// <returns>The authenticated session backed by its own HTTP client and cookies.</returns>
        private protected async Task<AuthenticatedUserSession> CreateAuthenticatedUserAsync(string? displayName = null)
        {
            var authenticatedUser = await AuthenticatedUserSession.CreateAsync(displayName);
            _authenticatedUsers.Add(authenticatedUser);
            return authenticatedUser;
        }
    }
}
