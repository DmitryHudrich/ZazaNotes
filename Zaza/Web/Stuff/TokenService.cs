using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Zaza.Web.DataBase.Entities;
using Zaza.Web.StorageInterfaces;

namespace Zaza.Web.Stuff;

internal static class TokenService {
    public static RefreshToken GenerateRefreshToken(uint expireFromDays) {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return new RefreshToken(Convert.ToBase64String(randomNumber), TimeSpan.FromDays(expireFromDays));
    }

    public static string MakeJwt(UserEntity user, HttpContext context, CookieOptions cookieOptions) {
        var logger = context.RequestServices.GetRequiredService<ILogger<JwtSecurityToken>>();

        var claims = new List<Claim>();
        if (user.AuthInfo.Web) {
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Login));
        }
        if (user.AuthInfo.Telegram) {
            claims.Add(new Claim(ClaimTypes.Sid, user.TelegramId.ToString()));
        }

        claims.Add(new Claim(ClaimTypes.SerialNumber, user.Id.ToString()));

        logger.LogDebug("Jwt token expiry: " + StaticStuff.JwtExpire);
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(StaticStuff.JwtExpire)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
               SecurityAlgorithms.HmacSha256));

        var cookies = context.Response.Cookies;

        var refresh = GenerateRefreshToken(180);
        if (user.Login != null) {
            cookies.Append("X-Username", user.Login, cookieOptions);
        }

        cookies.Append("X-Guid", user.Id.ToString());
        cookies.Append("X-Access", new JwtSecurityTokenHandler().WriteToken(jwt), cookieOptions);
        cookies.Append("X-Refresh", refresh.Data, cookieOptions);
        _ = context.RequestServices.GetRequiredService<IUserRepository>().ChangeRefreshAsync(user, refresh);
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
