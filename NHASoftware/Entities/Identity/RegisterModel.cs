using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
namespace NHA.Website.Software.Entities.Identity;
[AllowAnonymous]
public class RegisterModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<RegisterModel> _logger;
    private readonly IEmailSender _emailSender;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly RoleManager<IdentityRole> _roleManager;

    [BindProperty] public InputModel Input { get; set; } = new InputModel();
    public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();
    public string ReturnUrl { get; set; } = string.Empty;
    public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ILogger<RegisterModel> logger, IEmailSender emailSender, IWebHostEnvironment webHostEnvironment, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _logger = logger;
        _emailSender = emailSender;
        _webHostEnvironment = webHostEnvironment;
        _roleManager = roleManager;
    }

    public async Task OnGetAsync(string? returnUrl = null)
    {
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        if (ModelState.IsValid)
        {
            var user = new ApplicationUser()
            {
                UserName = Input.Email,
                Email = Input.Email,
                UserCash = 0.00,
                LastLoginDate = DateTime.Today,
                ProfilePicturePath = "DefaultProfilePicture.png",
                DisplayName = "Gangsta"
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("basic"))
                {
                    var createdRole = await _roleManager.CreateAsync(new IdentityRole("basic"));
                }

                if (!await _roleManager.RoleExistsAsync("admin"))
                {
                    var createdRole = await _roleManager.CreateAsync(new IdentityRole("admin"));
                }

                await _userManager.AddToRoleAsync(user, "basic");



                _logger.LogInformation("User created a new account with password.");
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = user.Id, code },
                    protocol: Request.Scheme);


                if (callbackUrl != null)
                {
                    await _emailSender.SendEmailAsync(Input.Email, "NHA Software Registration Confirmation",
                        $"Welcome to NHA Industries social media site! " +
                        $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");
                }


                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                }
                else
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "home");
                }
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        // If we got this far, something failed, redisplay form
        return Page();
    }

    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
            MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
