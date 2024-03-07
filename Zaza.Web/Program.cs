using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Zaza.Web.DataBase.Repository;
using Microsoft.AspNetCore.Authentication;
using Zaza.Web.Stuff;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder();
// аутентификация с помощью куки
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            // указывает, будет ли валидироваться издатель при валидации токена
            ValidateIssuer = true,
            // строка, представляющая издателя
            ValidIssuer = AuthOptions.ISSUER,
            // будет ли валидироваться потребитель токена
            ValidateAudience = true,
            // установка потребителя токена
            ValidAudience = AuthOptions.AUDIENCE,
            // будет ли валидироваться время существования
            ValidateLifetime = true,
            // установка ключа безопасности
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            // валидация ключа безопасности
            ValidateIssuerSigningKey = true,
        };
    });

builder.Services.AddCors();

var app = builder.Build();

app.UseAuthentication();   // добавление middleware аутентификации 
app.UseAuthorization();   // добавление middleware авторизации 
app.UseCors(options => {
    options.AllowAnyHeader();
    options.AllowAnyMethod();
    options.AllowAnyOrigin();
});

var repository = new UserRepository();
var cookieOptions = new CookieOptions { HttpOnly = true };

app.MapPost("/reg", (UserDTORecieve user) => {
    if (!repository.Add(user)) {
        return Results.Unauthorized();
    };
    return Results.Ok();
});

app.MapPost("/login", (HttpContext context, UserDTORecieve userDTO) => {
    string username = userDTO.Username;
    string password = userDTO.Password;

    var user = repository.Users.FirstOrDefault(u => u.Login == username && u.Password == password);

    var cookies = context.Response.Cookies;

    return MakeJwt(user, cookies, cookieOptions);
});

app.MapGet("/refresh", (HttpContext context) => {
    var username = context.Request.Cookies["X-Username"];
    var refresh = context.Request.Cookies["X-Refresh"] ?? string.Empty;
    var user = repository.Find(refresh);
    if (user == null) {
        return "саси";
    }
    return MakeJwt(user, context.Response.Cookies, cookieOptions);
});

app.MapGet("/logout", async (HttpContext context) => {
    await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
});

app.MapGet("/", [Authorize] (HttpContext context) => $"Hello {context.User.FindFirstValue(ClaimTypes.Name)}");

app.Run();

string MakeJwt(UserEntity? user, IResponseCookies cookies, CookieOptions cookieOptions) {
    var claims = new List<Claim> { new Claim(ClaimTypes.Name, user!.Login) };
    ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");
    var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            claims: claims,
            expires: DateTime.UtcNow.Add(TimeSpan.FromSeconds(5)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

    var refresh = TokenService.GenerateRefreshToken(180);
    cookies.Append("X-Username", user.Login, cookieOptions);
    cookies.Append("X-Access", new JwtSecurityTokenHandler().WriteToken(jwt), cookieOptions);
    cookies.Append("X-Refresh", refresh.Data, cookieOptions);
    repository.ChangeRefresh(user, refresh);
    return new JwtSecurityTokenHandler().WriteToken(jwt);
}
