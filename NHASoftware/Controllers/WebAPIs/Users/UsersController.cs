using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.DBContext;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Entities.Session;
using NHA.Website.Software.Entities.Social_Entities;
using NHA.Website.Software.Services.SessionHistory;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;

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
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;

        public UsersController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IActiveSessionTracker sessionTracker,
            ILogger<UsersController> logger,
            IOptionsMonitor<CookieAuthenticationOptions> cookieOptions,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _sessionTracker = sessionTracker;
            _logger = logger;
            _applicationCookieOptions = cookieOptions.Get(IdentityConstants.ApplicationScheme);
            _emailSender = emailSender;
            _context = context;
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
        [HttpPost("register")]
        [Consumes("application/json")]
        public Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            return RegisterCore(request);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [Consumes("application/x-www-form-urlencoded", "multipart/form-data")]
        public Task<IActionResult> RegisterForm([FromForm] RegisterUserRequest request)
        {
            return RegisterCore(request);
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

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody] DeleteUserRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            var requirePassword = await _userManager.HasPasswordAsync(user);
            if (requirePassword && string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest(new { message = "Password is required." });
            }

            if (requirePassword && !await _userManager.CheckPasswordAsync(user, request.Password!))
            {
                return BadRequest(new { message = "Incorrect password." });
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var userPostIds = await _context.Set<Post>()
                .Where(x => x.UserId == userId && x.Id.HasValue)
                .Select(x => x.Id!.Value)
                .ToListAsync();

            if (userPostIds.Count > 0)
            {
                var childPosts = await _context.Set<Post>()
                    .Where(x => x.ParentPostId.HasValue && userPostIds.Contains(x.ParentPostId.Value))
                    .ToListAsync();

                foreach (var childPost in childPosts)
                {
                    childPost.ParentPostId = null;
                }

                var postImages = await _context.Set<PostImage>()
                    .Where(x => x.PostId.HasValue && userPostIds.Contains(x.PostId.Value))
                    .ToListAsync();

                var reportedPosts = await _context.Set<ReportedPost>()
                    .Where(x => (x.PostId.HasValue && userPostIds.Contains(x.PostId.Value)) || x.UserId == userId)
                    .ToListAsync();

                var hiddenPosts = await _context.Set<HiddenPost>()
                    .Where(x => userPostIds.Contains(x.PostId))
                    .ToListAsync();

                var userPosts = await _context.Set<Post>()
                    .Where(x => x.UserId == userId)
                    .ToListAsync();

                if (postImages.Count > 0)
                {
                    _context.Set<PostImage>().RemoveRange(postImages);
                }

                if (reportedPosts.Count > 0)
                {
                    _context.Set<ReportedPost>().RemoveRange(reportedPosts);
                }

                if (hiddenPosts.Count > 0)
                {
                    _context.Set<HiddenPost>().RemoveRange(hiddenPosts);
                }

                _context.Set<Post>().RemoveRange(userPosts);
                await _context.SaveChangesAsync();
            }

            var sessionHistoryEvents = await _context.Set<SessionHistoryEvent>()
                .Where(x => x.userId == userId)
                .ToListAsync();

            if (sessionHistoryEvents.Count > 0)
            {
                _context.Set<SessionHistoryEvent>().RemoveRange(sessionHistoryEvents);
                await _context.SaveChangesAsync();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return ValidationProblem(ModelState);
            }

            await _signInManager.SignOutAsync();
            _logger.LogInformation("User with ID '{UserId}' deleted themselves through api/users.", userId);

            return Ok(new { message = "User deleted successfully.", userId });
        }

        private async Task<IActionResult> RegisterCore(RegisterUserRequest request)
        {
            var normalizedEmail = request.Email.Trim();
            var existingUser = await _userManager.FindByEmailAsync(normalizedEmail);

            if (existingUser != null)
                return Conflict(new { message = "A user with that email already exists." });

            var user = new ApplicationUser
            {
                UserName = normalizedEmail,
                Email = normalizedEmail,
                DisplayName = string.IsNullOrWhiteSpace(request.DisplayName)
                    ? "Anonymous Gangsta"
                    : request.DisplayName.Trim(),
                DateJoined = DateTime.UtcNow,
                LastLoginDate = DateTime.UtcNow,
            };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }

                return ValidationProblem(ModelState);
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId, code, returnUrl = request.ReturnUrl },
                protocol: Request.Scheme);

            if (!string.IsNullOrWhiteSpace(callbackUrl))
            {
                await _emailSender.SendEmailAsync(normalizedEmail, "Confirm your email",
                    $" Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            }

            _logger.LogInformation("User account created through api/users/register for {Email}.", normalizedEmail);

            if (!_userManager.Options.SignIn.RequireConfirmedAccount)
            {
                await _signInManager.SignInAsync(user, isPersistent: request.RememberMe);
            }

            return Created($"/api/users/{userId}", new
            {
                message = _userManager.Options.SignIn.RequireConfirmedAccount
                    ? "User created successfully. Confirm the email address before signing in."
                    : "User created and authenticated successfully.",
                userId,
                username = user.UserName,
                email = user.Email,
                displayName = user.DisplayName,
                requiresConfirmedAccount = _userManager.Options.SignIn.RequireConfirmedAccount,
                authenticationCookie = new
                {
                    name = _applicationCookieOptions.Cookie.Name,
                    set = Response.Headers.SetCookie.Count > 0,
                }
            });
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

        public class RegisterUserRequest
        {
            [Required]
            [EmailAddress]
            public required string Email { get; set; }

            [StringLength(20)]
            public string? DisplayName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            public required string Password { get; set; }

            [Required]
            [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
            public required string ConfirmPassword { get; set; }

            public bool RememberMe { get; set; }

            public string? ReturnUrl { get; set; }
        }

        public class AuthenticateUserRequest
        {
            public required string UserNameOrEmail { get; set; }
            public required string Password { get; set; }
            public bool RememberMe { get; set; }
        }

        public class DeleteUserRequest
        {
            public string? Password { get; set; }
        }

    }
}
