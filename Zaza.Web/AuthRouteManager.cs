using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Zaza.Web.DataBase.Repository;
using Zaza.Web.Stuff;

namespace Zaza.Web;

internal class AuthRouteManager(WebApplication app, UserRepository repository) {
    public void SetRoute() {
        app.MapPost("/auth/reg", (UserDTORecieve user) => {
            if (!repository.Add(user)) {
                return Results.Unauthorized();
            };
            return Results.Ok();
        });

        app.MapPost("/auth/login", (HttpContext context, UserDTORecieve userDTO) => {
            string username = userDTO.Username;
            string password = userDTO.Password;

            var user = repository.Users.FirstOrDefault(u => u.Login == username && u.Password == password);

            return MakeJwt(user, context, StaticStuff.SecureCookieOptions);
        });

        app.MapGet("/auth/refresh", (HttpContext context) => {
            var username = context.Request.Cookies["X-Username"];
            var refresh = context.Request.Cookies["X-Refresh"] ?? string.Empty;
            var user = repository.Find(refresh);
            if (user == null) {
                return "саси";
            }
            return MakeJwt(user, context, StaticStuff.SecureCookieOptions);
        });

        app.MapGet("/auth/logout", (HttpContext context) => {
            var cookies = context.Response.Cookies;

            cookies.Append("X-Username", "");
            cookies.Append("X-Access", "");
            cookies.Append("X-Refresh", "");

            return Results.Ok();
        });


    }

    private string MakeJwt(UserEntity? user, HttpContext context, CookieOptions cookieOptions) {
        var claims = new List<Claim> { new Claim(ClaimTypes.Name, user!.Login) };
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
        var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: claims,
                expires: DateTime.UtcNow.Add(TimeSpan.FromSeconds(10)),
                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));

        System.Console.WriteLine(DateTime.UtcNow.Add(TimeSpan.FromSeconds(10)));
        var cookies = context.Response.Cookies;

        var refresh = TokenService.GenerateRefreshToken(180);
        cookies.Append("X-Username", user.Login, cookieOptions);
        cookies.Append("X-Access", new JwtSecurityTokenHandler().WriteToken(jwt), cookieOptions);
        cookies.Append("X-Refresh", refresh.Data, cookieOptions);
        repository.ChangeRefresh(user, refresh);
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}
