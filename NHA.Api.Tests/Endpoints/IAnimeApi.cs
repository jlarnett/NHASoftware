using NHA.Website.Software.Controllers.WebAPIs.Anime;
using NHA.Website.Software.Entities.Anime;
using Refit;

namespace NHA.Api.Tests.Endpoints;

public interface IAnimeApi
{
    [Get("/api/AnimePages/list?pageNumber={pageNumber}")]
    Task<ApiResponse<List<AnimePage>>> GetAnimePagesAsync(int pageNumber);

    [Get("/api/AnimePages?id={id}")]
    Task<ApiResponse<AnimePage>> GetAnimePageAsync(int? id);

    [Post("/api/AnimePages")]
    Task<ApiResponse<AnimePage>> CreateAnimePageAsync([Body(BodySerializationMethod.Serialized)] CreateAnimePageRequest request);
}