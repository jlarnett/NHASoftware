using System.Security.Claims;

namespace NHASoftware.HelperClasses
{
    public interface IWarden
    {
        bool IsForumAdmin(ClaimsPrincipal User);
    }
}