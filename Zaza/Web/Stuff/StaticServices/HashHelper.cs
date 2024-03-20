using System.Security.Cryptography;
using System.Text;

namespace Zaza.Web.Stuff.StaticServices;

internal static class HashHelper {
    public static string Hash(string text) {
        var data = SHA256.HashData(Encoding.UTF8.GetBytes(text));

        var sBuilder = new StringBuilder();

        for (var i = 0; i < data.Length; i++) {
            _ = sBuilder.Append(data[i].ToString("x2"));
        }

        return sBuilder.ToString();
    }

    public static bool Verify(string text, string hash) => Hash(text) == hash;
}
