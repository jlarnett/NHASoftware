using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Services.SessionHistory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NHA.Website.Software.Controllers.WebAPIs.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IActiveSessionTracker _sessionTracker;
        private readonly ILogger<UsersController> _logger;
        private readonly CookieAuthenticationOptions _applicationCookieOptions;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IActiveSessionTracker sessionTracker,
            ILogger<UsersController> logger,
            IOptionsMonitor<CookieAuthenticationOptions> cookieOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _sessionTracker = sessionTracker;
            _logger = logger;
            _applicationCookieOptions = cookieOptions.Get(IdentityConstants.ApplicationScheme);
        }

        [HttpGet("search")]
        public IActionResult SearchUsers(string q)
        {
            var users = _userManager.Users
                .Where(u => u.DisplayName!.StartsWith(q) || u.Email!.StartsWith(q))
                .Select(u => new { id = u.Id, username = u.UserName })
                .Take(10)
                .ToList();

            return Ok(users);
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        [Consumes("application/json")]
        public Task<IActionResult> Authenticate([FromBody] AuthenticateUserRequest request)
        {
            return AuthenticateCore(request);
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        [Consumes("application/x-www-form-urlencoded", "multipart/form-data")]
        public Task<IActionResult> AuthenticateForm([FromForm] AuthenticateUserRequest request)
        {
            return AuthenticateCore(request);
        }

        private async Task<IActionResult> AuthenticateCore(AuthenticateUserRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserNameOrEmail) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("A username/email and password are required.");

            var normalizedInput = request.UserNameOrEmail.Trim();
            var user = await _userManager.FindByEmailAsync(normalizedInput)
                ?? await _userManager.FindByNameAsync(normalizedInput);

            if (user == null)
                return Unauthorized(new { message = "Invalid login attempt." });

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                user.LastLoginDate = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                if (!string.IsNullOrWhiteSpace(user.Email))
                {
                    await _sessionTracker.CreateLoginEvent(user.Email);
                }

                _logger.LogInformation("User authenticated through api/users/authenticate.");

                return Ok(new
                {
                    message = "Authentication successful.",
                    userId = user.Id,
                    username = user.UserName,
                    email = user.Email,
                    displayName = user.DisplayName,
                    biography = user.Biography,
                    lastLoginDate = user.LastLoginDate,
                    isPersistent = request.RememberMe,
                    authenticationCookie = new
                    {
                        name = _applicationCookieOptions.Cookie.Name,
                        set = Response.Headers.SetCookie.Count > 0,
                    }
                });
            }

            if (result.RequiresTwoFactor)
                return Unauthorized(new { message = "Two-factor authentication is required.", requiresTwoFactor = true });

            if (result.IsLockedOut)
                return Unauthorized(new { message = "User account is locked out.", isLockedOut = true });

            if (result.IsNotAllowed)
                return Unauthorized(new { message = "User account is not allowed to sign in. Confirm the account first.", isNotAllowed = true });

            return Unauthorized(new { message = "Invalid login attempt." });
        }

        [HttpPost("edit_bio")]
        public async Task<IActionResult> Edit([FromBody] EditBiographyRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.UserId))
                return BadRequest("Invalid user ID.");

            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
                return NotFound("User not found.");

            user.Biography = request.Biography.Trim();
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { message = "Biography updated successfully." });
        }

        public class EditBiographyRequest
        {
            public required string UserId { get; set; }
            public required string Biography { get; set; }
        }

        public class AuthenticateUserRequest
        {
            public required string UserNameOrEmail { get; set; }
            public required string Password { get; set; }
            public bool RememberMe { get; set; }
        }

    }
}
