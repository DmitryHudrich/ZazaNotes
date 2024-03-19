using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Zaza.Web.Stuff;

internal static class StaticStuff {
    static readonly public string MongoStingEnvName = "MONGO_URI";
    static readonly public string MongoStingDefault = "mongodb://localhost:27017";

    static readonly public CookieOptions SecureCookieOptions = new CookieOptions {
        HttpOnly = true,
        SameSite = SameSiteMode.Strict
    };

    static readonly public Action<JwtBearerOptions> JwtBearerOptions = options => {
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

    static public double JwtExpire;
}


