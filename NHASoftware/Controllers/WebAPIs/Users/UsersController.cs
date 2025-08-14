using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NHA.Website.Software.Entities.Identity;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NHA.Website.Software.Controllers.WebAPIs.Users
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UsersController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
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

    }
}
