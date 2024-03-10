﻿using System.Security.Claims;
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
                string err = $"User {context.GetName()} isn't found in db.";
                logger.LogDebug(new UserNotFoundException(nameof(user)), err);
                return Results.BadRequest(err);
            }

            var dto = new UserBodyResponse(user.Login, user.Info);
            return Results.Json(dto);
        });

        app.MapDelete("/user", [Authorize] (ILogger<RouteManager> logger, HttpContext context, NoteRepository noteRepository) => {
            var userStatus = repository.DeleteByLogin(context.GetName());
            var notesStatus = noteRepository.DeleteNotesByLogin(context.GetName());
            if (!userStatus || notesStatus == 0) {
                string err = $"User {context.GetName()} wasn't found or his don't have notes";
                logger.LogDebug(new EnitityNotFoundException($"User: {userStatus} Note: {notesStatus}"), err);
                return Results.BadRequest(err);
            }
            return Results.Ok();
        });

        app.MapPut("/user", [Authorize] (ILogger<RouteManager> logger, HttpContext context, UserInfo info, NoteRepository noteRepository) => {
            var user = context.GetName();
            return repository.ChangeInfo(user, info) ? Results.Ok() : handle();
            IResult handle() {
                string err = $"User {context.GetName()} wasn't changed";
                logger.LogDebug(err);
                return Results.BadRequest(err);
            }
        });

        app.MapGet("/user/notes", [Authorize] (NoteRepository notesRep, HttpContext context) => {
            string login = context.GetName();
            var notes = notesRep.GetNotes(login);
            return Results.Json(notes);
        });

        app.MapPost("/user/notes", [Authorize] (ILogger<RouteManager> logger, HttpContext context, NoteDTO note, NoteRepository notes) => {
            var res = Results.Ok();
            var username = context.GetName();
            var newNote = notes.AddNote(username, note.Title, note.Text);
            if (!newNote) {
                logger.LogDebug($"{username}: note wasn't add");
                return Results.BadRequest("Note wasn't add");
            }
            logger.LogDebug($"{username}: new note");
            return Results.Ok();
        });

        app.MapDelete("/user/notes/{id:guid}", [Authorize] (ILogger<RouteManager> logger, HttpContext context, Guid id, NoteRepository repository) => {
            var status = repository.DeleteNote(id);

            if (!status) {
                string err = $"{context.GetName}: note wasn't deleted";
                logger.LogDebug(err);
                return Results.BadRequest(err);
            }
            return Results.Ok();
        });

        app.MapPut("/user/notes", [Authorize] (ILogger<RouteManager> logger, HttpContext context, ChangedNoteDTO dto, NoteRepository notes) => {
            var status = notes.ChangeNote(dto);

            if (!status) {
                string err = $"{context.GetName}: note wasn't changed";
                logger.LogDebug(err);
                return Results.BadRequest(err);
            }
            return Results.Ok();

        });

    }

    private void Auth(UserRepository repository) {
        app.MapPost("/auth/reg", (ILogger<RouteManager> logger, UserMainDTO user) => {
            if (string.IsNullOrWhiteSpace(user.Password)) {
                string err = $"{user.Login}: account don't create, because password must contain more then zero symbols lol ";
                logger.LogDebug(new ArgumentException(nameof(user.Password)), err);
                return Results.BadRequest(err);
            }
            if (!repository.Add(user)) {
                string err = $"{user} wasn't added to database";
                logger.LogDebug(err);
                return Results.Unauthorized();
            };
            return Results.Ok();
        });

        app.MapPost("/auth/login", (ILogger<RouteManager> logger, UserLoginRequestDTO loginRequest, HttpContext context) => {
            var login = loginRequest.Login;
            var password = loginRequest.Password;

            var user = repository.FindByLogin(login);
            if (user == null || user.Password != password) {
                string err = $"User {user} wasn't found or password is incorrect";
                logger.LogDebug(err);
                Results.BadRequest(err);
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
