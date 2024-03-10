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

/* TODO:
 * users crud:
 * + create (registration)
 * + get 
 * + update
 * + delete
 * notes crud:
 * + get all notes
 * + make note
 * + change
 * + delete
 */

internal sealed class RouteManager(WebApplication app) {
    public void SetEndpoints() {
        var repository = app.Services.GetRequiredService<UserRepository>();
        Auth(repository);

        app.MapGet("/user", [Authorize] (ILogger<RouteManager> logger, HttpContext context) => {
            var user = repository.FindByLogin(context.GetName());
            if (user == null) {
                const string err = "Authorized user isn't fount in database";
                logger.LogWarning(new ArgumentNullException(nameof(user)), err);
                return Results.Problem(detail: err);
            }

            var dto = new UserBodyResponse(user.Login, user.Info);
            return Results.Json(dto);
        });

        app.MapDelete("/user", [Authorize] (HttpContext context, NoteRepository noteRepository) => {
            var userStatus = repository.DeleteByLogin(context.GetName());
            var notesStatus = noteRepository.DeleteNotesByLogin(context.GetName());
            if (userStatus && notesStatus != 0) {
                return Results.Ok();
            }
            return Results.BadRequest();
        });

        app.MapPut("/user", [Authorize] (HttpContext context, UserInfo info, NoteRepository noteRepository) => {
            var user = context.GetName();
            return repository.ChangeInfo(user, info) ? Results.Ok() : Results.BadRequest();
        });

        app.MapGet("/user/notes", [Authorize] (NoteRepository notesRep, HttpContext context) => {
            string login = context.GetName();
            var notes = notesRep.GetNotes(login);
            return Results.Json(notesRep.GetNotes(login));
        });

        app.MapPost("/user/notes", [Authorize] (ILogger<RouteManager> logger, HttpContext context, NoteDTO note, NoteRepository notes) => {
            var res = Results.Ok();
            var username = context.GetName();
            var newNote = notes.AddNote(username, note.Title, note.Text);

            if (newNote) {
                logger.LogDebug($"{username}: new note");
                return Results.Ok();
            }

            logger.LogDebug($"{username}: not isn't add");
            return Results.BadRequest("Note isn't add");
        });

        app.MapDelete("/user/notes/{id:guid}", [Authorize] (HttpContext conext, Guid id, NoteRepository repository) => {
            repository.DeleteNote(id);
        });

        app.MapPut("/user/notes", [Authorize] (HttpContext context, ChangedNoteDTO dto, NoteRepository notes) => {
            notes.ChangeNote(dto);
        });

    }

    private void Auth(UserRepository repository) {
        app.MapPost("/auth/reg", (ILogger<RouteManager> logger, UserMainDTO user) => {
            //logger.LogDebug("POST: /auth/reg");
            if (user.Password == "") {
                return Results.BadRequest();
            }
            logger.LogDebug($"Reg request: {user.ToString()}");
            if (!repository.Add(user)) {
                return Results.Unauthorized();
            };
            logger.LogTrace("Reg user ok");
            return Results.Ok();
        });

        app.MapPost("/auth/login", (ILogger<RouteManager> logger, UserLoginRequestDTO loginRequest, HttpContext context) => {
            var login = loginRequest.Login;
            var password = loginRequest.Password;

            var user = repository.FindByLogin(login);
            logger.LogTrace("User " + user?.Login);
            if (user == null || user.Password != password) {
                Results.Unauthorized();
            }

            return TokenService.MakeJwt(user!, context, StaticStuff.SecureCookieOptions);
        });

        app.MapGet("/auth/refresh", (HttpContext context) => {
            var username = context.Request.Cookies["X-Username"];
            var refresh = context.Request.Cookies["X-Refresh"] ?? string.Empty;
            var user = repository.FindByRefresh(refresh);
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
