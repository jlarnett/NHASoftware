using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace NHASoftware.Models.IdentityModels
{
    [AllowAnonymous]
    public abstract class LoginModel : PageModel
    {
        public List<AuthenticationScheme> ExternalLogins { get; set; }
        public string ReturnUrl { get; set; }

        public LoginModel()
        {
            
        }
    }
}
