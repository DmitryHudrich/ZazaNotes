namespace Zaza.Web.DataBase;

internal record class UserEntity(Guid Guid, UserInfo Info, string Login, string Password, RefreshToken RefreshToken) {
    public static UserEntity Empty { get; } = new UserEntity(Guid.Empty, new UserInfo(""), "", "", new RefreshToken("", TimeSpan.Zero));
    public DateTime Registration { get; init; } = DateTime.Now;
}

internal record class NoteEntity(Guid Guid, string OwnerLogin, UserInfo OwnerInfo, DateTime Creation, string Title = "", string Text = "") {
    public DateTime LastChange { get; init; } = DateTime.Now;
};

internal record class UserInfo(
        string FirstName, string LastName = "", string Description = "", string Country = "", string photo = "");

internal record class RefreshToken(string Data, TimeSpan expire);
