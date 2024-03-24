
using Zaza.Web.StorageInterfaces;
using Zaza.Web.Stuff.DTO.Request;
using Zaza.Web.Stuff.InteractLogic.InteractServices;

namespace Zaza.Web.Stuff.InteractLogic;

internal enum InteractEvent {
    AUTHORIZATION,
    CREATION,
    DELETION,
    CHANGE,
    RECEIVING
}

internal record class InteractResult(bool Success, InteractEvent Event, string? Error = default);
internal record class InteractResult<T>(bool Success, InteractEvent Event, T? Data = default, string? Error = default);

internal abstract class InteractAbstract(ILogger<InteractAbstract> logger, RepositoryContainer repositories) {
    protected ILogger<InteractAbstract> Logger => logger;
    protected IUserRepository UserRepository => repositories.ForUser;
    protected INoteRepository NoteRepository => repositories.ForNotes;
}

internal sealed class AuthInteractions(ILogger<AuthInteractions> logger, RepositoryContainer repositoryContainer) :
    InteractAbstract(logger, repositoryContainer) {

    public async Task<InteractResult<PasswordQuality>> RegisterUser(UserMainDTO userDTO) {
        var passwordQuality = PasswordQuality.STRONG;
        var status = true;
        if (!State.DisablePasswordValidation) {
            passwordQuality = AuthHelper.ValidatePassword(userDTO.Password);
        }
        if (!await UserRepository.AddAsync(userDTO) && passwordQuality != PasswordQuality.BAD) {
            var err = $"{userDTO} wasn't added to database";
            status = false;
            logger.LogDebug(err);
        };

        var res = passwordQuality switch {
            PasswordQuality.STRONG => new InteractResult<PasswordQuality>(status, InteractEvent.AUTHORIZATION, passwordQuality),
            PasswordQuality.GOOD => new InteractResult<PasswordQuality>(status, InteractEvent.AUTHORIZATION, passwordQuality),
            PasswordQuality.WEAK => new InteractResult<PasswordQuality>(status, InteractEvent.AUTHORIZATION, passwordQuality),
            PasswordQuality.BAD => new InteractResult<PasswordQuality>(status, InteractEvent.AUTHORIZATION, passwordQuality, "Bad password"),
            _ => throw new ArgumentException("Unknown password quality"),
        };

        return res;
    }


    public async Task<InteractResult> LoginUser(UserLoginRequestDTO userDTO) {
        if (!State.DisablePasswordValidation) {

        }
        if (string.IsNullOrWhiteSpace(userDTO.Password)) {
            var err = $"{userDTO.Login}: account didn't create, because password must contain more than zero symbols lol ";
        }
        throw new NotImplementedException();
    }
}
