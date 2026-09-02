namespace NHA.Api.Tests.Setup;

/// <summary>
/// Provides shared HTTP client and API client behavior for isolated API test sessions.
/// </summary>
internal abstract class ApiSessionBase
{
    /// <summary>
    /// Holds the HTTP client that owns session cookies and sends requests for this session.
    /// </summary>
    protected readonly HttpClient HttpClient;

    /// <summary>
    /// Initializes the session with a dedicated HTTP client and matching Refit APIs.
    /// </summary>
    /// <param name="httpClient">The HTTP client that defines the session's cookie and auth scope.</param>
    protected ApiSessionBase(HttpClient httpClient)
    {
        HttpClient = httpClient;
        var apiClients = ApiTestEnvironment.CreateApiClients(httpClient);
        AnimeApi = apiClients.AnimeApi;
        SearchApi = apiClients.SearchApi;
        SocialApi = apiClients.SocialApi;
        UsersApi = apiClients.UsersApi;
    }

    /// <summary>
    /// Gets the anime API client for this session.
    /// </summary>
    public Endpoints.IAnimeApi AnimeApi { get; }

    /// <summary>
    /// Gets the search API client for this session.
    /// </summary>
    public Endpoints.ISearchApi SearchApi { get; }

    /// <summary>
    /// Gets the social API client for this session.
    /// </summary>
    public Endpoints.ISocialApi SocialApi { get; }

    /// <summary>
    /// Gets the users API client for this session.
    /// </summary>
    public Endpoints.IUsersApi UsersApi { get; }

    /// <summary>
    /// Fetches the antiforgery token for the current session and target page.
    /// </summary>
    /// <param name="path">The relative page path used to retrieve the token.</param>
    /// <returns>The decoded antiforgery token value.</returns>
    public Task<string> GetAntiforgeryTokenAsync(string path = "/api/antiforgery") =>
        ApiTestEnvironment.GetAntiforgeryTokenAsync(HttpClient, path);

    /// <summary>
    /// Posts form data for the current session while automatically sending an antiforgery token.
    /// </summary>
    /// <param name="requestUri">The relative endpoint that receives the form submission.</param>
    /// <param name="formValues">The form fields to post.</param>
    /// <param name="antiforgeryPath">The relative page used to obtain the antiforgery token.</param>
    /// <returns>The HTTP response from the form post.</returns>
    public Task<HttpResponseMessage> PostFormWithAntiforgeryAsync(
        string requestUri,
        IEnumerable<KeyValuePair<string, string>> formValues,
        string antiforgeryPath = "/api/antiforgery") =>
        ApiTestEnvironment.PostFormWithAntiforgeryAsync(HttpClient, requestUri, formValues, antiforgeryPath);
}
