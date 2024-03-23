using Zaza.Web.Stuff.StaticServices;

namespace Zaza.Web.DataBase.Entities;

internal class Password(string password) {
    public string Hash { get; set; } = HashHelper.Hash(password);

    public override bool Equals(object? obj) {
        return obj is Password password && HashHelper.Verify(password.Hash, Hash);
    }

    public override int GetHashCode() => Hash.GetHashCode();
}

