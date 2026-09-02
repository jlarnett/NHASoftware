using NHA.Website.Software.Entities.Anime;
using Refit;

namespace NHA.Api.Tests.Endpoints;

public interface IAnimeApi
{
    [Get("/api/AnimePages?pageNumber={pageNumber}")]
    Task<ApiResponse<List<AnimePage>>> GetAnimePagesAsync(int pageNumber);
}