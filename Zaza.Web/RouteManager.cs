using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;

using Zaza.Web.DataBase;
using Zaza.Web.DataBase.Repository;
using Zaza.Web.Stuff;
using Zaza.Web.Stuff.DTO.Request;
using Zaza.Web.Stuff.DTO.Response;

namespace Zaza.Web;

internal static class RoutingHelper {
    public static string GetName(this HttpContext context) => context.User.FindFirstValue(ClaimTypes.Name) ?? throw new ArgumentNullException(nameof(GetName));
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
        app.MapPost("/user/notes", [Authorize]
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

        app.MapDelete("/user/notes/{id:guid}", [Authorize]
        async (ILogger<RouteEndpoint> logger, HttpContext context, Guid id, INoteRepository repository) => {
            var status = await repository.DeleteNoteAsync(id);

            if (!status) {
                string err = $"{context.GetName}: note wasn't deleted";
                logger.LogDebug(err);
                return Results.BadRequest(err);
            }
            return Results.Ok();
        });

        app.MapPut("/user/notes", [Authorize]
        async (ILogger<RouteEndpoint> logger, HttpContext context, ChangedNoteDTO dto, INoteRepository notes) => {
            var status = await notes.ChangeNoteAsync(dto, context.GetName());

            if (!status) {
                string err = $"{context.GetName}: note wasn't changed";
                logger.LogDebug(err);
                return Results.BadRequest(err);
            }
            return Results.Ok();

        });
    }

    private static void User() {
        app.MapGet("/user", [Authorize]
        async (IUserRepository repository, ILogger<RouteEndpoint> logger, HttpContext context) => {
            var user = await repository.FindByLoginAsync(context.GetName());
            if (user == null) {
                string err = $"User {context.GetName()} isn't found in db.";
                logger.LogDebug(new UserNotFoundException(nameof(user)), err);
                return Results.BadRequest(err);
            }

            var dto = new UserBodyResponse(user.Login, user.Info);
            return Results.Json(dto);
        });

        app.MapDelete("/user", [Authorize]
        async (IUserRepository repository, ILogger<RouteEndpoint> logger, HttpContext context, INoteRepository noteRepository) => {
            var userStatus = await repository.DeleteByLoginAsync(context.GetName());
            if (!userStatus) {
                string err = $"User {context.GetName()} wasn't found or his don't have notes";
                logger.LogDebug(new EnitityNotFoundException($"User: {userStatus}"), err);
                return Results.BadRequest(err);
            }
            return Results.Ok();
        });

        app.MapPut("/user", [Authorize]
        async (IUserRepository repository, ILogger<RouteEndpoint> logger, HttpContext context, UserInfo info, INoteRepository noteRepository) => {
            var user = context.GetName();
            return await repository.ChangeInfoAsync(user, info) ? Results.Ok() : handle();
            IResult handle() {
                string err = $"User {context.GetName()} wasn't changed";
                logger.LogDebug(err);
                return Results.BadRequest(err);
            }
        });

        app.MapGet("/user/notes", [Authorize]
        async (INoteRepository notesRep, HttpContext context) => {
            string login = context.GetName();
            var notes = await notesRep.GetNotesAsync(login);
            return Results.Json(notes);
        });
    }

    private static void Auth() {
        app.MapPost("/auth/password", [Authorize]
        async (HttpContext context, ChangePasswordDTO user, IUserRepository repository) => {
            if (!await repository.ChangePasswordAsync(context.GetName(), user.OldPassword, user.NewPassword)) {
                return Results.BadRequest(""); // TODO: дописать ошибку
            }
            return Results.Ok();
        });

        app.MapPost("/auth/reg", async (IUserRepository repository, ILogger<RouteEndpoint> logger, UserMainDTO user) => {
            if (string.IsNullOrWhiteSpace(user.Password)) {
                string err = $"{user.Login}: account don't create, because password must contain more then zero symbols lol ";
                logger.LogDebug(new ArgumentException(nameof(user.Password)), err);
                return Results.BadRequest(err);
            }
            if (!await repository.AddAsync(user)) {
                string err = $"{user} wasn't added to database";
                logger.LogDebug(err);
                return Results.Unauthorized();
            };
            return Results.Ok();
        });

        app.MapPost("/auth/login", async (IUserRepository repository, ILogger<RouteEndpoint> logger, UserLoginRequestDTO loginRequest, HttpContext context) => {
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

        app.MapGet("/auth/refresh", async (IUserRepository repository, HttpContext context) => {
            var username = context.Request.Cookies["X-Username"];
            var refresh = context.Request.Cookies["X-Refresh"] ?? string.Empty;
            var user = await repository.FindByRefreshAsync(refresh);
            if (user == null) {
                return "саси";
            }
            return TokenService.MakeJwt(user, context, StaticStuff.SecureCookieOptions);
        });

        app.MapGet("/auth/logout", (HttpContext context) => {
            var cookies = context.Response.Cookies;

            cookies.Append("X-Username", "");
            cookies.Append("X-Access", "");
            cookies.Append("X-Refresh", "");

            return Results.Ok();
        });
    }
}
