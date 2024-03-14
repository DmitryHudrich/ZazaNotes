using Zaza.Web.DataBase;
using Zaza.Web.DataBase.Repository;
using Zaza.Web.Stuff.DTO.Request;

namespace Zaza.Web;

internal sealed class NoteRepository(ILogger<NoteRepository> logger, IUserRepository userRepository) : INoteRepository {
    private static List<NoteEntity> notes = [];

    public async Task<bool> AddNoteAsync(string login, string title, string text) {
        var user = await GetUserAsync(login);
        notes.Add(new NoteEntity(Guid.NewGuid(), user.Login, user.Info, DateTime.Now, title, text));
        logger.LogDebug($"User {login} added a note: {title}");
        return true;
    }

    public async Task<int> DeleteNotesByLoginAsync(string login) => notes.RemoveAll(note => note.OwnerLogin == login);

    public async IAsyncEnumerable<NoteEntity> GetNotesAsync(string login) {
        if (await GetUserAsync(login) == UserEntity.Empty) {
            logger.LogDebug($"{login} don't exists");
        }
        foreach (var note in notes) {
            if (note.OwnerLogin == login) {
                yield return note;
            }
        }
    }

    public async Task<bool> DeleteNoteAsync(Guid id) {
        var note = notes.FirstOrDefault(note => note.Id == id);
        if (note == null) {
            logger.LogDebug("Note with guid:{id} don't exists");
            return false;
        }
        var res = notes.Remove(note);
        if (!res) {
            logger.LogDebug("Remove operation ne poluchilas");
            return false;
        }
        return true;
    }

    public async Task<bool> ChangeNoteAsync(ChangedNoteDTO newNote) {
        var note = notes.FirstOrDefault(n => n.Id == newNote.Guid);
        if (note == null) {
            logger.LogDebug($"Note was not found");
            return false;
        }
        note = note with { Creation = note.Creation, Title = newNote.Title, Text = newNote.Text };
        int i = notes.FindIndex(0, note => note.Id == newNote.Guid);
        notes[i] = note;
        logger.LogDebug("Added note: " + notes.FirstOrDefault(note => note.Id == newNote.Guid)?.ToString());
        return true;
    }

    private async Task<UserEntity> GetUserAsync(string login) {
        var user = await userRepository.FindByLoginAsync(login);
        if (user == null) {
            var err = $"{login} isn't exist";
            logger.LogDebug(err);
            return UserEntity.Empty;
        }
        return user;
    }
}
