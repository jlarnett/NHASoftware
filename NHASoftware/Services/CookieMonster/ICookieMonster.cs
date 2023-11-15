namespace NHA.Website.Software.Services.CookieMonster;
public interface ICookieMonster
{
    void CreateCookie(string key, string value, CookieOptions? options = null);
    string TryRetrieveCookie(string key);
}
