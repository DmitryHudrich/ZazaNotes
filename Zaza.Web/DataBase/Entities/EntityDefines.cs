using MongoDB.Bson.Serialization.Attributes;

namespace Zaza.Web.DataBase.Entities;

internal record class UserEntity(Guid Id, UserInfo Info, string Login, Password Password, RefreshToken RefreshToken) {
    public static UserEntity Empty { get; } = new UserEntity(Guid.Empty, new UserInfo(""), "", new Password(""), new RefreshToken("", TimeSpan.Zero));
    public DateTime Registration { get; init; } = DateTime.Now;
    public List<NoteEntity> Notes { get; set; } = [];
}

internal record class NoteEntity(Guid Id, string Title = "", string Text = "") {
    public DateTime LastChange { get; init; } = DateTime.Now;
};

internal class Password(string password) {
    public string Hash { get; set; } = HashHelper.Hash(password);

    public override bool Equals(object? obj) {
        return obj is Password password && HashHelper.Verify(password.Hash, Hash);
    }

    public override int GetHashCode() => Hash.GetHashCode();
}

internal record class UserInfo(
        string FirstName, string LastName = "", string Description = "", string Country = "", string Photo = "");

internal record class RefreshToken(string Data, TimeSpan Expire);
