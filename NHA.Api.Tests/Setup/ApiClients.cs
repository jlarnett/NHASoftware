using NHA.Api.Tests.Endpoints;
namespace NHA.Api.Tests.Setup;

/// <summary>
/// Groups the typed Refit clients used by the API test infrastructure.
/// </summary>
internal sealed record ApiClients
{
    /// <summary>
    /// Initializes a new grouped set of typed API clients.
    /// </summary>
    /// <param name="animeApi">The Refit client for anime endpoints.</param>
    /// <param name="searchApi">The Refit client for search endpoints.</param>
    /// <param name="socialApi">The Refit client for social endpoints.</param>
    /// <param name="usersApi">The Refit client for user endpoints.</param>
    internal ApiClients(IAnimeApi animeApi, ISearchApi searchApi, ISocialApi socialApi, IUsersApi usersApi)
    {
        AnimeApi = animeApi;
        SearchApi = searchApi;
        SocialApi = socialApi;
        UsersApi = usersApi;
    }

    /// <summary>
    /// Gets the Refit client for anime endpoints.
    /// </summary>
    internal IAnimeApi AnimeApi { get; }

    /// <summary>
    /// Gets the Refit client for search endpoints.
    /// </summary>
    internal ISearchApi SearchApi { get; }

    /// <summary>
    /// Gets the Refit client for social endpoints.
    /// </summary>
    internal ISocialApi SocialApi { get; }

    /// <summary>
    /// Gets the Refit client for user endpoints.
    /// </summary>
    internal IUsersApi UsersApi { get; }
}
