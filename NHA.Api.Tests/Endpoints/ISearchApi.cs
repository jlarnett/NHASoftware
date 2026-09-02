using NHA.Website.Software.Controllers.WebAPIs.Search;
using Refit;

namespace NHA.Api.Tests.Endpoints
{
    public interface ISearchApi
    {
        [Get("/api/search/{searchTerm}")]
        Task<ApiResponse<SearchResponse>> GetSearchAsync(string searchTerm);
    }
}
