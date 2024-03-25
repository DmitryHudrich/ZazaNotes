using Zaza.Web.Stuff.DTO.Request;

namespace Zaza.Web.Stuff.InteractLogic.Auth;

internal sealed class AuthInteractions(ILogger<AuthInteractions> logger, RepositoryContainer repositoryContainer) :
    InteractAbstract(logger, repositoryContainer) {

    public async Task<InteractResult<PasswordQuality>> RegisterUser(UserMainDTO userDTO) {
        var passwordQuality = PasswordQuality.STRONG;
        var status = true;

        if (!State.DisablePasswordValidation) {
            passwordQuality = AuthHelper.ValidatePassword(userDTO.Password);
        }

        if (passwordQuality == PasswordQuality.BAD) {
            var err = $"{userDTO} password is too weak";
            status = false;
            logger.LogDebug(err);
        };

        if (!status || !await UserRepository.AddAsync(userDTO)) {
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
        if (string.IsNullOrWhiteSpace(userDTO.Password)) {
            var err = $"{userDTO.Login}: account didn't create, because password must contain more than zero symbols lol ";
        }
        throw new NotImplementedException();
    }
}
