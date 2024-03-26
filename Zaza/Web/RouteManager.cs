using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Zaza.Web.DataBase.Entities;
using Zaza.Web.DataBase.Repository;
using Zaza.Web.Exceptions;
using Zaza.Web.StorageInterfaces;
using Zaza.Web.Stuff;
using Zaza.Web.Stuff.DTO.Request;
using Zaza.Web.Stuff.DTO.Response;
using Zaza.Web.Stuff.InteractLogic.Auth;
using Zaza.Web.Stuff.InteractLogic.User;

namespace Zaza.Web;

internal static class RoutingHelper {
    public static string GetName(this HttpContext context) => context.User.FindFirstValue(ClaimTypes.NameIdentifier) ??
        throw new ArgumentNullException(nameof(context));

    public static Guid GetId(this HttpContext context) => context.User.FindFirstValue(ClaimTypes.SerialNumber) is not string o ?
      throw new ArgumentNullException(nameof(context)) : Guid.Parse(o);
}

internal static class RouteManager {
    private static IEndpointRouteBuilder app = null!;
    public static void SetEndpoints(IEndpointRouteBuilder webApp, ILogger logger) {
        app = webApp;
        Auth();
        User();
        Notes();
        Telegram();
        Health();
        if (State.TestApi) {
            logger.LogWarning("TestApi enabled");
            Testing();
        }
    }

    // TODO: API REFACTOR IGNORE
    private static void Health() {
        _ = app.MapGet("/health/ping", () => "pong");
    }

    private static void Telegram() {
        // TODO: API REFACTOR
        _ = app.MapPost("/telegram/auth", async (IUserRepository repository, ILogger<RouteEndpoint> logger, HttpContext context, UserTelegramDTO dto) => {
            var code = await repository.AddAsync(dto) ? 201 : 200;
            var user = await repository.FindByFilterAsync(FindFilter.TELEGRAM_ID, dto.Id);
            var jwt = TokenService.MakeJwt(user!, context, StaticStuff.SecureCookieOptions);
            return Results.Json(data: jwt, statusCode: code);
        });
    }

    private static void Notes() {
        // TODO: API REFACTOR
        _ = app.MapPost("/user/notes", [Authorize]
        async (ILogger<RouteEndpoint> logger, HttpContext context, NoteDTO note, INoteRepository notes) => {
            var res = Results.Ok();
            var userId = context.GetId();
            var newNote = await notes.AddNoteAsync(userId, note.Title, note.Text);
            if (!newNote) {
                logger.LogDebug($"{userId}: note wasn't add");
                return Results.BadRequest("Note wasn't add");
            }
            logger.LogDebug($"{userId}: new note");
            return Results.Created();
        });

        // TODO: API REFACTOR
        _ = app.MapDelete("/user/notes/{id:guid}", [Authorize]
        async (ILogger<RouteEndpoint> logger, HttpContext context, Guid id, INoteRepository repository) => {
            var status = await repository.DeleteNoteAsync(id);

            if (!status) {
                var err = $"{context.GetName}: note wasn't deleted";
                logger.LogDebug(err);
                return Results.NotFound(err);
            }
            return Results.NoContent();
        });

        // TODO: API REFACTOR
        _ = app.MapPut("/user/notes", [Authorize]
        async (ILogger<RouteEndpoint> logger, HttpContext context, ChangedNoteDTO dto, INoteRepository notes) => {
            var status = await notes.ChangeNoteAsync(dto, context.GetId());

            if (!status) {
                var err = $"{context.GetName}: note wasn't changed";
                logger.LogDebug(err);
                return Results.BadRequest(err);
            }
            return Results.Created();

        });
    }

    private static void Testing() {
        _ = app.MapPost("/testing/flush", (ILogger<RouteEndpoint> logger) => {
            logger.LogWarning(
                    "-------------------------------\n" +
                    "FLUSHING DATABASE\n" +
                    "-------------------------------");
            _ = new DataBase.MongoService().Users.DeleteMany(new MongoDB.Bson.BsonDocument());
            return Results.Accepted();
        });
    }

    private static void User() {
        _ = app.MapGet("/user", [Authorize]
        async (UserInteractions intetactions, ILogger<RouteEndpoint> logger, HttpContext context) => {
            var res = await intetactions.PullUserAsync(context);
            return res.Success
                ? Results.Json(data: res.Data, statusCode: 200)
                : Results.Json(data: res.Data, statusCode: 400);
        });

        _ = app.MapDelete("/user", [Authorize]
        async (HttpContext context, UserInteractions interactions) => {
            var res = await interactions.DeleteUserAsync(context);
            return res.Success
                ? Results.NoContent()
                : Results.BadRequest(res.Error);
        });

        // TODO: API REFACTOR
        _ = app.MapPut("/user", [Authorize]
        async (IUserRepository repository, ILogger<RouteEndpoint> logger, HttpContext context, UserInfo info, INoteRepository noteRepository) => {
            var userId = context.GetId();
            return await repository.ChangeInfoAsync(userId, info) ? Results.Ok() : Handle();
            IResult Handle() {
                var err = $"User {context.GetName()} wasn't changed";
                logger.LogDebug(err);
                return Results.BadRequest(err);
            }
        });

        // TODO: API REFACTOR
        _ = app.MapGet("/user/notes", [Authorize]
        async (INoteRepository notesRep, HttpContext context) => {
            var userId = context.GetId();
            var notes = await notesRep.GetNotesAsync(userId);
            return Results.Json(notes);
        });
    }

    private static void Auth() {
        _ = app.MapPost("/auth/password", [Authorize]
        async (HttpContext context, ChangePasswordDTO user, AuthInteractions interactions) => {
            var res = await interactions.ChangeUserPasswordAsync(context, user);
            return res.Success
                ? Results.Ok()
                : Results.BadRequest(res.Error);
        });

        _ = app.MapPost("/auth/reg", async (UserMainDTO user, AuthInteractions interactions) => {
            var res = await interactions.RegisterUserAsync(user);
            return res.Success
                ? Results.Json(data: res.Data, statusCode: 201)
                : Results.Json(data: res.Data, statusCode: 400);
        });

        _ = app.MapPost("/auth/login", async (AuthInteractions interactions, UserLoginRequestDTO loginRequest, HttpContext context) => {
            var res = await interactions.LoginUserAsync(context, loginRequest);
            return res.Success
                ? Results.Json(data: res.Data, statusCode: 200)
                : Results.Json(data: res.Data, statusCode: 401);
        });

        // TODO: API REFACTOR
        _ = app.MapGet("/auth/refresh", async (IUserRepository repository, HttpContext context) => {
            var username = context.Request.Cookies["X-Username"];
            var refresh = context.Request.Cookies["X-Refresh"] ?? string.Empty;
            var user = await repository.FindByFilterAsync(FindFilter.REFRESH, refresh);
            return user == null ? "саси" : TokenService.MakeJwt(user, context, StaticStuff.SecureCookieOptions);
        });

        // TODO: API REFACTOR
        _ = app.MapGet("/auth/logout", (HttpContext context) => {
            var cookies = context.Response.Cookies;

            cookies.Append("X-Username", "");
            cookies.Append("X-Access", "");
            cookies.Append("X-Refresh", "");
            cookies.Append("X-Guid", "");

            return Results.NoContent();
        });
    }
}
