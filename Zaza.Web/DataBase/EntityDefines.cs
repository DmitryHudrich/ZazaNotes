namespace Zaza.Web.DataBase;

internal record class UserEntity(Guid Id, UserInfo Info, string Login, string Password, RefreshToken RefreshToken) {
    public static UserEntity Empty { get; } = new UserEntity(Guid.Empty, new UserInfo(""), "", "", new RefreshToken("", TimeSpan.Zero));
    public DateTime Registration { get; init; } = DateTime.Now;
    public List<NoteEntity> Notes { get; set; } = [];
}

internal record class NoteEntity(Guid Id, string Title = "", string Text = "") {
    public DateTime LastChange { get; init; } = DateTime.Now;
};

internal record class UserInfo(
        string FirstName, string LastName = "", string Description = "", string Country = "", string photo = "");

internal record class RefreshToken(string Data, TimeSpan expire);
