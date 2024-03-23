/* using Zaza.Web.StorageInterfaces; */
/* using Zaza.Web.Stuff.DTO.Request; */
/**/
/* namespace Zaza.Web.Logic; */
/**/
/* internal enum Results { */
/*     Created, */
/*     Changed, */
/*     Patched, */
/*     Deleted, */
/*     Returned, */
/*     Error, */
/* } */
/**/
/* internal sealed record class InteractionResult<T>(Results Type, T? Data = default, string? Error = null); */
/**/
/* internal abstract class InteractionAbstract(ILogger<InteractionAbstract> logger) { */
/*     protected ILogger<InteractionAbstract> Logger => logger; */
/* } */
/**/
/* internal sealed class Auth(ILogger<Auth> logger) : InteractionAbstract(logger) { */
/*     public async Task RegistrationAsync(IUserRepository repository, UserMainDTO user) { */
/*         if (string.IsNullOrWhiteSpace(user.Password)) { */
/*             var err = $"{user.Login}: account didn't create, because password must contain more than zero symbols lol "; */
/*             logger.LogDebug(new ArgumentException(nameof(user.Password)), err); */
/*             return Results.BadRequest(err); */
/*         } */
/*         if (!await repository.AddAsync(user)) { */
/*             var err = $"{user} wasn't added to database"; */
/*             logger.LogDebug(err); */
/*             return Results.Unauthorized(); */
/*         }; */
/*         return Results.Created(); */
/*     } */
/* } */
