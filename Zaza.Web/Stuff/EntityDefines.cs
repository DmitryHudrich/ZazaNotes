using System.Security.Cryptography;

namespace Zaza.Web.Stuff;

internal record class UserEntity(Guid Guid, string Login, string Password, RefreshToken RefreshToken);

internal record class RefreshToken(string Data, TimeSpan expire);

internal static class TokenService {
    public static RefreshToken GenerateRefreshToken(uint expireFromDays) {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create()) {
            rng.GetBytes(randomNumber);
            return new RefreshToken(Convert.ToBase64String(randomNumber), TimeSpan.FromDays(expireFromDays));
        }
    }
}
