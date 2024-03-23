namespace Zaza.Web.DataBase.Entities;

internal record class UserEntity {
#pragma warning disable CS8618 // this ctor is internal and must be called by other ctors
    private UserEntity(Guid id, UserInfo info) {
        Id = id;
        Info = info;
    }
#pragma warning restore CS8618

    public UserEntity(Guid id, UserInfo info, string login, string password, RefreshToken refresh) : this(id, info) {
        Login = login;
        Password = new Password(password);
        Refresh = refresh;
    }
    public UserEntity(Guid id, UserInfo info, ulong telegramId) : this(id, info) {
        TelegramId = telegramId;
    }

    public DateTime Registration { get; init; } = DateTime.Now;
    public List<NoteEntity> Notes { get; set; } = [];
    public AuthInfo AuthInfo { get; set; } = new AuthInfo();
    public Guid Id { get; init; }
    public UserInfo Info { get; init; }
    public string Login { get; init; }
    public Password Password { get; init; }
    public RefreshToken Refresh { get; init; }
    public ulong TelegramId { get; init; }
}

internal class AuthInfo {
    public bool Web { get; set; } = false;
    public bool Telegram { get; set; } = false;
}

// TODO: realize grpc
internal enum RegistrationPlace {
    FROM_API,
    TELEGRAM,
    UNKNOWN,
}

