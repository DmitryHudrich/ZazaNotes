namespace Zaza.Web.Stuff.InteractLogic.InteractServices;

internal enum PasswordQuality {
    BAD,
    WEAK,
    GOOD,
    STRONG
}

internal static class AuthHelper {
    public static PasswordQuality ValidatePassword(string password) {
        var qualityPoints = 0;
        if (password.Length > 8) {
            qualityPoints++;
        }
        if (password.Any(char.IsUpper)) {
            qualityPoints++;
        }
        if (password.Any(char.IsLower)) {
            qualityPoints++;
        }
        if (password.Any(char.IsDigit)) {
            qualityPoints++;
        }
        var res = qualityPoints switch {
            1 => PasswordQuality.BAD,
            2 => PasswordQuality.WEAK,
            3 => PasswordQuality.GOOD,
            4 => PasswordQuality.STRONG,
            _ => PasswordQuality.STRONG
        };
        return res;
    }
}
