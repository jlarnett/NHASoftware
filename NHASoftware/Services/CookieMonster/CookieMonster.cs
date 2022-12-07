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

    public void CreateCookie(string key, string value, CookieOptions options = null)
    {
        var cookie = _contextAccessor.HttpContext.Request.Cookies[key];

        if (options == null)
        {
            options = new CookieOptions();
            options.Expires = DateTime.Now.AddDays(1);
        }

        if (cookie == null)
        {
            _contextAccessor.HttpContext.Response.Cookies.Append(key, value, options);
        }
    }
}
