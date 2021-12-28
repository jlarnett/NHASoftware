using Microsoft.AspNetCore.Identity;

namespace NHASoftware.Models
{
    public class ApplicationUser : IdentityUser
    {
        public double UserCash { get; set; }
    }
}
