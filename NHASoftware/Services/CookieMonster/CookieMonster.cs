namespace NHA.Website.Software.Services.CookieMonster;
public class CookieMonster : ICookieMonster
{
    private readonly IHttpContextAccessor _contextAccessor;

    public CookieMonster(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }
    public string TryRetrieveCookie(string key)
    {
        if (_contextAccessor.HttpContext == null)
        {
            return string.Empty;
        }
        var cookie = _contextAccessor.HttpContext.Request.Cookies[key];
        return cookie ?? string.Empty;
    }

    /// <summary>
    /// Creates a new cookie for the accessing user
    /// </summary>
    /// <param name="key">Cookie string Key</param>
    /// <param name="value">Cookie string value</param>
    /// <param name="options"></param>
    public void CreateCookie(string key, string value, CookieOptions? options = null)
    {
        if (_contextAccessor.HttpContext != null)
        {
            var cookie = _contextAccessor.HttpContext.Request.Cookies[key];

            if (options == null)
            {
                options = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(365)
                };
            }

            if (cookie == null)
            {
                _contextAccessor.HttpContext.Response.Cookies.Append(key, value, options);
            }
        }
    }
}
