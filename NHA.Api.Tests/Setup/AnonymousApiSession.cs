using System.Net;
namespace NHA.Api.Tests.Setup;

/// <summary>
/// Represents an isolated anonymous API test session with its own cookie container.
/// </summary>
internal sealed class AnonymousApiSession : ApiSessionBase, IAsyncDisposable
{
    /// <summary>
    /// Creates a new anonymous session backed by a dedicated HTTP client.
    /// </summary>
    /// <param name="allowAutoRedirect"><c>true</c> to follow redirects automatically; otherwise, <c>false</c>.</param>
    public AnonymousApiSession(bool allowAutoRedirect = true) : base(ApiTestEnvironment.CreateHttpClient(new CookieContainer(), allowAutoRedirect))
    {
    }

    /// <summary>
    /// Disposes the session HTTP client and releases any associated resources.
    /// </summary>
    public ValueTask DisposeAsync()
    {
        HttpClient.Dispose();
        return ValueTask.CompletedTask;
    }
}
