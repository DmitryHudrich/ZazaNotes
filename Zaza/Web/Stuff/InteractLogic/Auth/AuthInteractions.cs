using Zaza.Web.DataBase.Repository;
using Zaza.Web.Stuff.DTO.Request;
using Zaza.Web.Stuff.DTO.Response;
using Zaza.Web.Stuff.StaticServices;

namespace Zaza.Web.Stuff.InteractLogic.Auth;

internal sealed class AuthInteractions(ILogger<AuthInteractions> logger, RepositoryContainer repositoryContainer) :
    InteractAbstract(logger, repositoryContainer) {
    private const InteractEvent INTERACT_EVENT = InteractEvent.AUTHORIZATION;

    public async Task<InteractResult<PasswordQuality>> RegisterUserAsync(UserMainDTO userDTO) {
        var passwordQuality = PasswordQuality.STRONG;
        var status = true;

        if (!State.DisablePasswordValidation) {
            passwordQuality = ValidatePassword(userDTO.Password);
        }
        status = await CheckPasswordQuality(userDTO, passwordQuality, status);

        var res = passwordQuality switch {
            PasswordQuality.STRONG => new InteractResult<PasswordQuality>(status, INTERACT_EVENT, passwordQuality),
            PasswordQuality.GOOD => new InteractResult<PasswordQuality>(status, INTERACT_EVENT, passwordQuality),
            PasswordQuality.WEAK => new InteractResult<PasswordQuality>(status, INTERACT_EVENT, passwordQuality),
            PasswordQuality.BAD => new InteractResult<PasswordQuality>(status, INTERACT_EVENT, passwordQuality, "Bad password"),
            _ => throw new ArgumentException("Unknown password quality"),
        };

        return res;
    }

    public async Task<InteractResult<JwtToken>> LoginUserAsync(HttpContext context, UserLoginRequestDTO userDTO) {
        var user = await UserRepository.FindByFilterAsync(FindFilter.LOGIN, userDTO.Login);
        return user is null
            ? ResultHelper("User not found.")
            : !HashHelper.Verify(userDTO.Password, user.Password.Hash)
            ? ResultHelper("Wrong password.")
            : new InteractResult<JwtToken>(Success: true, Event: INTERACT_EVENT, Data: TokenService.MakeJwt(user, context, StaticStuff.SecureCookieOptions));

        InteractResult<JwtToken> ResultHelper(string err) {
            logger.LogDebug($"{nameof(LoginUserAsync)}: {err}");
            return new InteractResult<JwtToken>(Success: false, Event: INTERACT_EVENT, Error: err);
        }
    }

    public async Task<InteractResult> ChangeUserPasswordAsync(HttpContext context, ChangePasswordDTO dto) => await ChangeUserPasswordAsync(context.GetId(), dto);
    public async Task<InteractResult> ChangeUserPasswordAsync(Guid id, ChangePasswordDTO dto) {
        logger.LogDebug($"{nameof(ChangeUserPasswordAsync)}: changing password for {id}");
        var passwordQuality = ValidatePassword(dto.NewPassword);
        if (passwordQuality == PasswordQuality.BAD) {
            var err = $"Password is too weak";
            logger.LogDebug($"{nameof(ChangeUserPasswordAsync)}: {err}");
            return new InteractResult(Success: false, Event: InteractEvent.CHANGE, Error: err);
        }
        return await UserRepository.ChangePasswordAsync(id, dto.OldPassword, dto.NewPassword)
                        ? new InteractResult(Success: true, Event: InteractEvent.CHANGE)
                        : new InteractResult(Success: false, Event: InteractEvent.CHANGE, Error: $"User: {id} is not found or password is wrong");
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

    private PasswordQuality ValidatePassword(string password) {
        var qualityPoints = 0;
        var rules = new List<bool> {
            password.Length > 8,
            password.Any(char.IsUpper),
            password.Any(char.IsLower),
            password.Any(char.IsDigit)
        };
        foreach (var rule in rules) {
            if (rule) {
                qualityPoints++;
            }
        }

        logger.LogTrace($"{nameof(ValidatePassword)}: {password} {qualityPoints}");

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
