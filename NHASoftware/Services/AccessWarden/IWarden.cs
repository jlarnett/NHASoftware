using System.Security.Claims;

namespace NHASoftware.Services.AccessWarden
{
    public interface IWarden
    {
        bool IsForumAdmin(ClaimsPrincipal user);
        bool IsAdmin(ClaimsPrincipal user);

    }
}