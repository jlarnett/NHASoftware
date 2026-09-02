namespace NHA.Api.Tests.Endpoints;

public class RegisterUserRequest
{
    public required string Email { get; set; }
    public string? DisplayName { get; set; }
    public required string Password { get; set; }
    public required string ConfirmPassword { get; set; }
    public bool RememberMe { get; set; }
    public string? ReturnUrl { get; set; }
}

public class RegisterUserResponse
{
    public string Message { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public bool RequiresConfirmedAccount { get; set; }
    public AuthenticationCookieResponse AuthenticationCookie { get; set; } = new();
}

public class AuthenticateUserRequest
{
    public required string UserNameOrEmail { get; set; }
    public required string Password { get; set; }
    public bool RememberMe { get; set; }
}

public class AuthenticateUserResponse
{
    public string Message { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? DisplayName { get; set; }
    public string? Biography { get; set; }
    public DateTime? LastLoginDate { get; set; }
    public bool IsPersistent { get; set; }
    public AuthenticationCookieResponse AuthenticationCookie { get; set; } = new();
}

public class DeleteUserRequest
{
    public string? Password { get; set; }
}

public class DeleteUserResponse
{
    public string Message { get; set; } = string.Empty;
    public string? UserId { get; set; }
}

public class AuthenticationCookieResponse
{
    public string? Name { get; set; }
    public bool Set { get; set; }
}
