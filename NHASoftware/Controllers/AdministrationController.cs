using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Views.ViewModels;
namespace NHA.Website.Software.Controllers;
public class AdministrationController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public IActionResult CreateRole()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
    {
        if (ModelState.IsValid)
        {
            IdentityRole identityRole = new IdentityRole()
            {
                Name = model.RoleName
            };

            IdentityResult result = await _roleManager.CreateAsync(identityRole);

            if (result.Succeeded)
            {
                return RedirectToAction("index", "Home");
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        return View(model);
    }

    [HttpGet]
    [Authorize(Roles = "admin")]
    public async Task<IActionResult> Users()
    {
        var users = await _userManager.Users
            .OrderBy(user => user.DisplayName ?? user.UserName ?? user.Email)
            .ThenBy(user => user.Email)
            .ToListAsync();

        var model = new AdminUsersViewModel
        {
            Users = new List<AdminUserOverviewViewModel>(users.Count)
        };

        foreach (var user in users)
        {
            var roles = (await _userManager.GetRolesAsync(user))
                .OrderBy(role => role)
                .ToList();

            var directClaims = (await _userManager.GetClaimsAsync(user))
                .Where(claim => !string.Equals(claim.Type, System.Security.Claims.ClaimTypes.Role, StringComparison.OrdinalIgnoreCase))
                .Select(FormatPermission)
                .ToList();

            var rolePermissions = new List<string>();

            foreach (var roleName in roles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);

                if (role is null)
                {
                    continue;
                }

                var claims = await _roleManager.GetClaimsAsync(role);

                rolePermissions.AddRange(claims
                    .Where(claim => !string.Equals(claim.Type, System.Security.Claims.ClaimTypes.Role, StringComparison.OrdinalIgnoreCase))
                    .Select(FormatPermission));
            }

            model.Users.Add(new AdminUserOverviewViewModel
            {
                Id = user.Id,
                DisplayName = string.IsNullOrWhiteSpace(user.DisplayName) ? user.UserName ?? user.Email ?? user.Id : user.DisplayName,
                UserName = user.UserName,
                Email = user.Email,
                ProfilePicturePath = user.ProfilePicturePath,
                DateJoined = user.DateJoined,
                Roles = roles,
                Permissions = directClaims
                    .Concat(rolePermissions)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .OrderBy(permission => permission)
                    .ToList()
            });
        }

        return View(model);
    }

    private static string FormatPermission(System.Security.Claims.Claim claim)
    {
        if (string.IsNullOrWhiteSpace(claim.Value))
        {
            return claim.Type;
        }

        return $"{claim.Type}: {claim.Value}";
    }
}
