namespace NHASoftware.Services.CookieMonster;

public class CookieMonster : ICookieMonster
{
    private readonly IHttpContextAccessor _contextAccessor;

    public CookieMonster(IHttpContextAccessor contextAccessor)
    {
        this._contextAccessor = contextAccessor;
    }
    public string TryRetrieveCookie(string key)
    {
        return _contextAccessor.HttpContext.Request.Cookies[key];
    }

    /// <summary>
    /// Creates a new cookie for the accessing user
    /// </summary>
    /// <param name="key">Cookie string Key</param>
    /// <param name="value">Cookie string value</param>
    /// <param name="options"></param>
    public void CreateCookie(string key, string value, CookieOptions options = null)
    {
        var cookie = _contextAccessor.HttpContext.Request.Cookies[key];

        if (options == null)
        {
            options = new CookieOptions();
            options.Expires = DateTime.Now.AddDays(365);
        }

        if (cookie == null)
        {
            _contextAccessor.HttpContext.Response.Cookies.Append(key, value, options);
        }
    }

}
