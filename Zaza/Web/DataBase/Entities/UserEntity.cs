namespace Zaza.Web.DataBase.Entities;

internal record class UserEntity(
    Guid Id,
    UserInfo Info,
    string Login,
    Password Password,
    long TelegramId,
    RefreshToken RefreshToken) {
    public DateTime Registration { get; init; } = DateTime.Now;
    public List<NoteEntity> Notes { get; set; } = [];
    public AuthInfo AuthInfo { get; set; } = new AuthInfo();

    public static UserEntity Empty { get; } = new UserEntity(
            Id: Guid.Empty,
            Info: new UserInfo(""),
            Login: "",
            Password: new Password(""),
            TelegramId: 0,
            RefreshToken: new RefreshToken("", TimeSpan.Zero));
}

internal class AuthInfo {
    public bool Web { get; set; } = false;
    public bool Telegram { get; set; } = false;
}

// TODO: realize grpc
internal enum RegistrationPlace {
    FROM_API,
    SERVICE,
    UNKNOWN,
}

