using System.Security.Claims;

namespace NHASoftware.Services.AccessWarden
{
    public interface IWarden
    {
        bool IsForumAdmin(ClaimsPrincipal User);
        bool IsAdmin(ClaimsPrincipal User);

    }
}