using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;

using Zaza.Web.DataBase.Entities;
using Zaza.Web.Exceptions;
using Zaza.Web.StorageInterfaces;
using Zaza.Web.Stuff;
using Zaza.Web.Stuff.DTO.Request;
using Zaza.Web.Stuff.DTO.Response;
using Zaza.Web.Stuff.StaticServices;

namespace Zaza.Web;

internal static class RoutingHelper {
    public static string GetName(this HttpContext context) => context.User.FindFirstValue(ClaimTypes.Name) ??
        throw new ArgumentNullException(nameof(context));
}

internal static class RouteManager {
    private static IEndpointRouteBuilder app = null!;
    public static void SetEndpoints(IEndpointRouteBuilder webApp) {
        app = webApp;
        Auth();
        User();
        Notes();
    }

    private static void Notes() {
        _ = app.MapPost("/user/notes", [Authorize]
        async (ILogger<RouteEndpoint> logger, HttpContext context, NoteDTO note, INoteRepository notes) => {
            var res = Results.Ok();
            var username = context.GetName();
            var newNote = await notes.AddNoteAsync(username, note.Title, note.Text);
            if (!newNote) {
                logger.LogDebug($"{username}: note wasn't add");
                return Results.BadRequest("Note wasn't add");
            }
            logger.LogDebug($"{username}: new note");
            return Results.Ok();
        });

        _ = app.MapDelete("/user/notes/{id:guid}", [Authorize]
        async (ILogger<RouteEndpoint> logger, HttpContext context, Guid id, INoteRepository repository) => {
            var status = await repository.DeleteNoteAsync(id);

            if (!status) {
                var err = $"{context.GetName}: note wasn't deleted";
                logger.LogDebug(err);
                return Results.BadRequest(err);
            }
            return Results.Ok();
        });

        _ = app.MapPut("/user/notes", [Authorize]
        async (ILogger<RouteEndpoint> logger, HttpContext context, ChangedNoteDTO dto, INoteRepository notes) => {
            var status = await notes.ChangeNoteAsync(dto, context.GetName());

            if (!status) {
                var err = $"{context.GetName}: note wasn't changed";
                logger.LogDebug(err);
                return Results.BadRequest(err);
            }
            return Results.Ok();

        });
    }

    private static void User() {
        _ = app.MapGet("/user", [Authorize]
        async (IUserRepository repository, ILogger<RouteEndpoint> logger, HttpContext context) => {
            var user = await repository.FindByLoginAsync(context.GetName());
            if (user == null) {
                var err = $"User {context.GetName()} wasn't found in db.";
                logger.LogDebug(new UserNotFoundException(nameof(user)), err);
                return Results.BadRequest(err);
            }

            var dto = new UserBodyResponse(user.Login, user.Info);
            return Results.Json(dto);
        });

        _ = app.MapDelete("/user", [Authorize]
        async (IUserRepository repository, ILogger<RouteEndpoint> logger, HttpContext context, INoteRepository noteRepository) => {
            var userStatus = await repository.DeleteByLoginAsync(context.GetName());
            if (!userStatus) {
                var err = $"User {context.GetName()} wasn't found or user doesn't have notes";
                logger.LogDebug(new EnitityNotFoundException($"User: {userStatus}"), err);
                return Results.BadRequest(err);
            }
            return Results.Ok();
        });

        _ = app.MapPut("/user", [Authorize]
        async (IUserRepository repository, ILogger<RouteEndpoint> logger, HttpContext context, UserInfo info, INoteRepository noteRepository) => {
            var user = context.GetName();
            return await repository.ChangeInfoAsync(user, info) ? Results.Ok() : Handle();
            IResult Handle() {
                var err = $"User {context.GetName()} wasn't changed";
                logger.LogDebug(err);
                return Results.BadRequest(err);
            }
        });

        _ = app.MapGet("/user/notes", [Authorize]
        async (INoteRepository notesRep, HttpContext context) => {
            var login = context.GetName();
            var notes = await notesRep.GetNotesAsync(login);
            return Results.Json(notes);
        });
    }

    private static void Auth() {
        _ = app.MapPost("/auth/password", [Authorize]
        async (HttpContext context, ChangePasswordDTO user, IUserRepository repository) => {
            return !await repository.ChangePasswordAsync(context.GetName(), user.OldPassword, user.NewPassword)
                ? Results.BadRequest($"User: {context.GetName()} is not found or password is wrong")
                : Results.Ok();
        });

        _ = app.MapPost("/auth/telegram", [Authorize]
        async (IUserRepository repository, HttpContext context, long id) => {
            var user = await repository.FindByLoginAsync(context.GetName());
            if (user == null) {
                return Results.BadRequest($"User: {context.GetName()} is not found");
            }
            await repository.ChangeTelegramId(context.GetName(), id);
            return Results.Ok();
        });

        _ = app.MapGet("/auth/telegram", [Authorize]
        async (IUserRepository repository, HttpContext context) => {
            var user = await repository.FindByLoginAsync(context.GetName());
            return user == null ? Results.BadRequest($"User: {context.GetName()} is not found") : Results.Json(user.TelegramId);
        });

        _ = app.MapPost("/auth/reg", async (IUserRepository repository, ILogger<RouteEndpoint> logger, UserMainDTO user) => {
            if (string.IsNullOrWhiteSpace(user.Password)) {
                var err = $"{user.Login}: account didn't create, because password must contain more than zero symbols lol ";
                logger.LogDebug(new ArgumentException(nameof(user.Password)), err);
                return Results.BadRequest(err);
            }
            if (!await repository.AddAsync(user)) {
                var err = $"{user} wasn't added to database";
                logger.LogDebug(err);
                return Results.Unauthorized();
            };
            return Results.Ok();
        });

        _ = app.MapPost("/auth/login", async (IUserRepository repository, ILogger<RouteEndpoint> logger, UserLoginRequestDTO loginRequest, HttpContext context) => {
            var login = loginRequest.Login;
            var password = loginRequest.Password;
            var user = await repository.FindByLoginAsync(login);
            string err;

            if (user == null) {
                err = $"User {login} wasn't found";
                logger.LogDebug(err);
                return Results.BadRequest(err);
            }
            if (!HashHelper.Verify(password, user.Password.Hash)) {
                err = $"User {login} password is wrong";
                return Results.BadRequest(err);
            }

            return Results.Json(
                    TokenService.MakeJwt(user!, context, StaticStuff.SecureCookieOptions));
        });

        _ = app.MapGet("/auth/refresh", async (IUserRepository repository, HttpContext context) => {
            var username = context.Request.Cookies["X-Username"];
            var refresh = context.Request.Cookies["X-Refresh"] ?? string.Empty;
            var user = await repository.FindByRefreshAsync(refresh);
            return user == null ? "саси" : TokenService.MakeJwt(user, context, StaticStuff.SecureCookieOptions);
        });

        _ = app.MapGet("/auth/logout", (HttpContext context) => {
            var cookies = context.Response.Cookies;

            cookies.Append("X-Username", "");
            cookies.Append("X-Access", "");
            cookies.Append("X-Refresh", "");

            return Results.Ok();
        });
    }
}
