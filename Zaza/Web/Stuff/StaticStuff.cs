using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Zaza.Web.Stuff;

internal static class StaticStuff {
    public static readonly string MongoStringEnvName = "MONGO_URI";
    public static readonly string MongoStringDefault = "mongodb://localhost:27017";

    public static readonly CookieOptions SecureCookieOptions = new CookieOptions {
        HttpOnly = true,
        SameSite = SameSiteMode.Strict
    };

    public static readonly Action<JwtBearerOptions> JwtBearerOptions = options => {
        options.TokenValidationParameters = new TokenValidationParameters {
            ClockSkew = TimeSpan.Zero,
            ValidateIssuer = true,
            ValidIssuer = AuthOptions.ISSUER,
            ValidateAudience = true,
            ValidAudience = AuthOptions.AUDIENCE,
            ValidateLifetime = true,
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            ValidateIssuerSigningKey = true,
        };
    };

    public static double JwtExpire;
}
