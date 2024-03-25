namespace Zaza.Web.Stuff.DTO.Response;

internal record class JwtToken(string Token) {
    public static implicit operator JwtToken(string str) => new JwtToken(str.Replace("\"", ""));
}
