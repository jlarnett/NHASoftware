using System.Security.Claims;
namespace NHA.Website.Software.Services.AccessWarden;
public interface IWarden
{
    bool IsForumAdmin(ClaimsPrincipal user);
    bool IsAdmin(ClaimsPrincipal user);

}
