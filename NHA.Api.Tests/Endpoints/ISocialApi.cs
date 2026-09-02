using NHA.Website.Software.ConsumableEntities.DTOs;
using NHA.Website.Software.Controllers.WebAPIs.Search;
using Refit;

namespace NHA.Api.Tests.Endpoints
{
    public interface ISocialApi
    {
        [Post("/api/posts/BasicPost")]
        Task<ApiResponse<SearchResponse>> CreateBasicPostAsync(PostDTO post);
    }
}
