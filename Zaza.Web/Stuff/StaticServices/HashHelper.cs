using System.Security.Cryptography;
using System.Text;

namespace Zaza.Web;

internal static class HashHelper {
    public static string Hash(string text) {
        using SHA256 shaHash = SHA256.Create();
        byte[] data = shaHash.ComputeHash(Encoding.UTF8.GetBytes(text));

        StringBuilder sBuilder = new StringBuilder();

        for (int i = 0; i < data.Length; i++) {
            sBuilder.Append(data[i].ToString("x2"));
        }

        return sBuilder.ToString();
    }

    public static bool Verify(string text, string hash) => Hash(text) == hash;
}
