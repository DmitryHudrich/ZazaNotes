using Zaza.Web.Stuff.DTO.Request;

namespace Zaza.Web.Stuff.InteractLogic.Auth;

internal sealed class AuthInteractions(ILogger<AuthInteractions> logger, RepositoryContainer repositoryContainer) :
    InteractAbstract(logger, repositoryContainer) {

    public async Task<InteractResult<PasswordQuality>> RegisterUser(UserMainDTO userDTO) {
        var passwordQuality = PasswordQuality.STRONG;
        var status = true;

        if (!State.DisablePasswordValidation) {
            passwordQuality = ValidatePassword(userDTO.Password);
        }
        status = await CheckPasswordQuality(userDTO, passwordQuality, status);

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

    private async Task<bool> CheckPasswordQuality(UserMainDTO userDTO, PasswordQuality passwordQuality, bool status) {
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
        return status;
    }

    private static PasswordQuality ValidatePassword(string password) {
        var qualityPoints = 0;
        var rules = new List<bool> {
            password.Length > 0,
            password.Any(char.IsUpper),
            password.Any(char.IsLower),
            password.Any(char.IsDigit)
        };

        foreach (var rule in rules) {
            if (rule) {
                qualityPoints++;
            }
        }

        var res = qualityPoints switch {
            1 => PasswordQuality.BAD,
            2 => PasswordQuality.WEAK,
            3 => PasswordQuality.GOOD,
            4 => PasswordQuality.STRONG,
            _ => PasswordQuality.STRONG
        };
        return res;
    }
}
