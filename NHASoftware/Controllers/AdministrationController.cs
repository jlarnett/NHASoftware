using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHA.Website.Software.Entities.Identity;
using NHA.Website.Software.Views.ViewModels;
namespace NHA.Website.Software.Controllers;
public class AdministrationController : Controller
{
    private readonly RoleManager<IdentityRole> _roleManager;
    public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
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
}
