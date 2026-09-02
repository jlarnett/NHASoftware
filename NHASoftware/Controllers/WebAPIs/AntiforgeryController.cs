using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NHA.Website.Software.Controllers.WebAPIs;

[ApiController]
[Route("api/[controller]")]
public class AntiforgeryController : ControllerBase
{
    private readonly IWebHostEnvironment _environment;
    private readonly IAntiforgery _antiforgery;

    public AntiforgeryController(IWebHostEnvironment environment, IAntiforgery antiforgery)
    {
        _environment = environment;
        _antiforgery = antiforgery;
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult Get()
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var tokens = _antiforgery.GetAndStoreTokens(HttpContext);

        return Ok(new
        {
            requestToken = tokens.RequestToken,
            formFieldName = tokens.FormFieldName,
            headerName = tokens.HeaderName,
        });
    }
}