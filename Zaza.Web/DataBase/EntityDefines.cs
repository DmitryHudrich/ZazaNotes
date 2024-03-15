using MongoDB.Bson.Serialization.Attributes;

namespace Zaza.Web.DataBase;

internal record class UserEntity(Guid Id, UserInfo Info, string Login, Password Password, RefreshToken RefreshToken) {
    public static UserEntity Empty { get; } = new UserEntity(Guid.Empty, new UserInfo(""), "", new Password(""), new RefreshToken("", TimeSpan.Zero));
    public DateTime Registration { get; init; } = DateTime.Now;
    public List<NoteEntity> Notes { get; set; } = [];
}

internal record class NoteEntity(Guid Id, string Title = "", string Text = "") {
    public DateTime LastChange { get; init; } = DateTime.Now;
};

internal class Password {
    [BsonElement]
    private string hash;

    public string Hash => hash;

    public Password(string password) {
        hash = HashHelper.Hash(password);
    }

    public override bool Equals(object? obj) {
        if (obj is not Password password) {
            return false;
        }

        if (!HashHelper.Verify(password.Hash, hash)) {
            return false;
        }
        return true;
    }

    public override int GetHashCode() => hash.GetHashCode();
}

internal record class UserInfo(
        string FirstName, string LastName = "", string Description = "", string Country = "", string photo = "");

internal record class RefreshToken(string Data, TimeSpan expire);
