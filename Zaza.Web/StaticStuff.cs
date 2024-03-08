namespace Zaza.Web;

public static class StaticStuff {
    static readonly public CookieOptions SecureCookieOptions = new CookieOptions { HttpOnly = true, SameSite = SameSiteMode.Strict };
}
