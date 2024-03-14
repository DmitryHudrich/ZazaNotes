using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Zaza.Web.DataBase;
using Zaza.Web.DataBase.Repository;

namespace Zaza.Web.Stuff;

internal static class TokenService {
    public static RefreshToken GenerateRefreshToken(uint expireFromDays) {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create()) {
            rng.GetBytes(randomNumber);
            return new RefreshToken(Convert.ToBase64String(randomNumber), TimeSpan.FromDays(expireFromDays));
        }
    }

    public static string MakeJwt(UserEntity user, HttpContext context, CookieOptions cookieOptions) {
        var logger = context.RequestServices.GetRequiredService<ILogger<JwtSecurityToken>>();

        var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.Login) };
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromHours(10)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));

        var cookies = context.Response.Cookies;

        var refresh = TokenService.GenerateRefreshToken(180);
        cookies.Append("X-Username", user.Login, cookieOptions);
        cookies.Append("X-Access", new JwtSecurityTokenHandler().WriteToken(jwt), cookieOptions);
        cookies.Append("X-Refresh", refresh.Data, cookieOptions);
        context.RequestServices.GetRequiredService<IUserRepository>().ChangeRefreshAsync(user, refresh);
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

}

