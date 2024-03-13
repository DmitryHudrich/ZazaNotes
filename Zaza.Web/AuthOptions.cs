using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Zaza.Web;

internal static class AuthOptions {
    public const string ISSUER = "BebraProvider";
    public const string AUDIENCE = "ZazaConsumer";
    const string KEY = "mysupersecret_secretsecretsecretkey!123";

    public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
}


