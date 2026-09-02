using HtmlAgilityPack;
using System.Net;
using NHA.Api.Tests.Endpoints;
using Refit;

namespace NHA.Api.Tests.Setup;

/// <summary>
/// Centralizes shared HTTP client, Refit client, and antiforgery helpers for API tests.
/// </summary>
internal static class ApiTestEnvironment
{
    private const string DefaultAntiforgeryPath = "/Identity/Account/Login";

    /// <summary>
    /// Resolves the API base URL from environment configuration or falls back to the local development URL.
    /// </summary>
    internal static string ApiBaseUrl =>
        Environment.GetEnvironmentVariable("NHA_API_BASE_URL")?.TrimEnd('/')
        ?? "https://localhost:44385";

    /// <summary>
    /// Stores cookies for the shared HTTP client used by non-isolated test flows.
    /// </summary>
    private static readonly CookieContainer SharedCookieContainer = new();

    /// <summary>
    /// Shared HTTP client for test requests that can safely reuse cookies and authentication state.
    /// </summary>
    internal static readonly HttpClient SharedHttpClient = CreateHttpClient(SharedCookieContainer);

    /// <summary>
    /// Creates an HTTP client configured for cookie-based API and Razor Pages interactions.
    /// </summary>
    /// <param name="cookieContainer">The cookie container that will persist cookies for the client.</param>
    /// <param name="allowAutoRedirect"><c>true</c> to automatically follow redirects; otherwise, <c>false</c>.</param>
    /// <returns>A configured HTTP client targeting the current API base URL.</returns>
    internal static HttpClient CreateHttpClient(CookieContainer cookieContainer, bool allowAutoRedirect = true)
    {
        return new HttpClient(new HttpClientHandler
        {
            CookieContainer = cookieContainer,
            UseCookies = true,
            AllowAutoRedirect = allowAutoRedirect,
        })
        {
            BaseAddress = new Uri(ApiBaseUrl)
        };
    }

    /// <summary>
    /// Creates a grouped set of Refit clients that share the same underlying HTTP client.
    /// </summary>
    /// <param name="httpClient">The HTTP client that all generated Refit APIs should use.</param>
    /// <returns>A bundle of typed Refit API clients.</returns>
    internal static ApiClients CreateApiClients(HttpClient httpClient)
    {
        return new ApiClients(
            RestService.For<IAnimeApi>(httpClient),
            RestService.For<ISearchApi>(httpClient),
            RestService.For<ISocialApi>(httpClient),
            RestService.For<IUsersApi>(httpClient));
    }

    /// <summary>
    /// Reads a page and extracts the antiforgery token value from its HTML markup.
    /// </summary>
    /// <param name="httpClient">The client whose cookies and base address should be used for the request.</param>
    /// <param name="path">The relative path that renders the antiforgery token.</param>
    /// <returns>The decoded antiforgery token value.</returns>
    internal static async Task<string> GetAntiforgeryTokenAsync(HttpClient httpClient, string path = DefaultAntiforgeryPath)
    {
        var html = await httpClient.GetStringAsync(path);
        var token = ExtractAntiforgeryToken(html);

        if (string.IsNullOrWhiteSpace(token) && !string.Equals(path, DefaultAntiforgeryPath, StringComparison.OrdinalIgnoreCase))
        {
            html = await httpClient.GetStringAsync(DefaultAntiforgeryPath);
            token = ExtractAntiforgeryToken(html);
            path = DefaultAntiforgeryPath;
        }

        if (string.IsNullOrWhiteSpace(token))
        {
            throw new InvalidOperationException($"Antiforgery token was not found at path '{path}'.");
        }

        return WebUtility.HtmlDecode(token);
    }

    private static string? ExtractAntiforgeryToken(string html)
    {
        var document = new HtmlDocument();
        document.LoadHtml(html);

        return document.DocumentNode
            .Descendants("input")
            .FirstOrDefault(node => string.Equals(node.GetAttributeValue("name", string.Empty), "__RequestVerificationToken", StringComparison.OrdinalIgnoreCase))
            ?.GetAttributeValue("value", null);
    }

    /// <summary>
    /// Posts URL-encoded form data while automatically attaching a matching antiforgery token.
    /// </summary>
    /// <param name="httpClient">The client whose cookies should be used for the antiforgery handshake and post.</param>
    /// <param name="requestUri">The relative endpoint that will receive the form post.</param>
    /// <param name="formValues">The form fields to submit.</param>
    /// <param name="antiforgeryPath">The relative path used to obtain the antiforgery token.</param>
    /// <returns>The response returned by the server.</returns>
    internal static async Task<HttpResponseMessage> PostFormWithAntiforgeryAsync(
        HttpClient httpClient,
        string requestUri,
        IEnumerable<KeyValuePair<string, string>> formValues,
        string antiforgeryPath = DefaultAntiforgeryPath)
    {
        var antiforgeryToken = await GetAntiforgeryTokenAsync(httpClient, antiforgeryPath);
        using var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
        {
            Content = new FormUrlEncodedContent(formValues)
        };

        request.Headers.Add("RequestVerificationToken", antiforgeryToken);
        return await httpClient.SendAsync(request);
    }
}
