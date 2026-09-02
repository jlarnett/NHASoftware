using Refit;

namespace NHA.Api.Tests.Endpoints;

public interface IUsersApi
{
    [Post("/api/users/register")]
    Task<ApiResponse<RegisterUserResponse>> RegisterAsync(RegisterUserRequest request);

    [Post("/api/users/register")]
    Task<ApiResponse<RegisterUserResponse>> RegisterFormAsync([Body(BodySerializationMethod.UrlEncoded)] RegisterUserRequest request);

    [Post("/api/users/authenticate")]
    Task<ApiResponse<AuthenticateUserResponse>> AuthenticateAsync(AuthenticateUserRequest request);

    [Post("/api/users/authenticate")]
    Task<ApiResponse<AuthenticateUserResponse>> AuthenticateFormAsync([Body(BodySerializationMethod.UrlEncoded)] AuthenticateUserRequest request);

    [Delete("/api/users")]
    Task<ApiResponse<DeleteUserResponse>> DeleteAsync([Body] DeleteUserRequest request);
}
