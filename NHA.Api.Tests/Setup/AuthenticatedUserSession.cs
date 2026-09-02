using System.Net;
using NHA.Api.Tests.Endpoints;

namespace NHA.Api.Tests.Setup;

/// <summary>
/// Represents an isolated authenticated API test user and its session-specific HTTP state.
/// </summary>
internal sealed class AuthenticatedUserSession : ApiSessionBase, IAsyncDisposable
{
    /// <summary>
    /// Prevents duplicate cleanup work when the session is disposed more than once.
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// Creates a new authenticated user session with a dedicated cookie container.
    /// </summary>
    /// <param name="email">The generated email address for the test user.</param>
    /// <param name="password">The password assigned to the test user.</param>
    private AuthenticatedUserSession(string email, string password) : base(ApiTestEnvironment.CreateHttpClient(new CookieContainer()))
    {
        Email = email;
        Password = password;
    }

    /// <summary>
    /// Gets the email address used by the generated test user.
    /// </summary>
    public string Email { get; }

    /// <summary>
    /// Gets the password used by the generated test user.
    /// </summary>
    public string Password { get; }

    /// <summary>
    /// Gets the application user identifier assigned after registration succeeds.
    /// </summary>
    public string? UserId { get; private set; }

    /// <summary>
    /// Creates, registers, and authenticates a new API test user session.
    /// </summary>
    /// <param name="displayName">Optional display name to assign during registration.</param>
    /// <returns>A fully initialized authenticated user session.</returns>
    public static async Task<AuthenticatedUserSession> CreateAsync(string? displayName = null)
    {
        const string password = "Aa1!Aa1!";
        var authenticatedUser = new AuthenticatedUserSession($"apitest_{Guid.NewGuid():N}@example.com", password);
        await authenticatedUser.InitializeAsync(displayName);
        return authenticatedUser;
    }

    /// <summary>
    /// Registers the generated user and ensures the session is authenticated.
    /// </summary>
    /// <param name="displayName">Optional display name to submit during registration.</param>
    private async Task InitializeAsync(string? displayName)
    {
        var registerResponse = await UsersApi.RegisterAsync(new RegisterUserRequest
        {
            Email = Email,
            DisplayName = displayName,
            Password = Password,
            ConfirmPassword = Password,
        });

        if (!registerResponse.IsSuccessStatusCode || string.IsNullOrWhiteSpace(registerResponse.Content?.UserId))
        {
            throw new InvalidOperationException($"Failed to create API test user. Status: {(int)registerResponse.StatusCode} {registerResponse.ReasonPhrase}");
        }

        UserId = registerResponse.Content!.UserId;

        if (registerResponse.Content.AuthenticationCookie.Set)
        {
            return;
        }

        if (!await AuthenticateAsync())
        {
            throw new InvalidOperationException($"Failed to authenticate API test user '{Email}'.");
        }
    }

    /// <summary>
    /// Authenticates the session using the generated user credentials.
    /// </summary>
    /// <returns><c>true</c> when authentication succeeds; otherwise, <c>false</c>.</returns>
    private async Task<bool> AuthenticateAsync()
    {
        var authenticateResponse = await UsersApi.AuthenticateAsync(new AuthenticateUserRequest
        {
            UserNameOrEmail = Email,
            Password = Password,
        });

        return authenticateResponse.IsSuccessStatusCode;
    }

    /// <summary>
    /// Attempts to delete the generated user and then disposes the session HTTP client.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        try
        {
            var deleteResponse = await UsersApi.DeleteAsync(new DeleteUserRequest
            {
                Password = Password
            });

            if (!deleteResponse.IsSuccessStatusCode && await AuthenticateAsync())
            {
                await UsersApi.DeleteAsync(new DeleteUserRequest
                {
                    Password = Password
                });
            }
        }
        catch (Exception e)
        {
            await Console.Error.WriteLineAsync($"Failed to clean up API test user '{Email}': {e}");
        }
        finally
        {
            HttpClient.Dispose();
        }
    }
}
