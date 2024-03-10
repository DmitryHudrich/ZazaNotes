using Zaza.Web.DataBase;
using Zaza.Web.DataBase.Repository;
using Zaza.Web.Stuff.DTO.Request;

namespace Zaza.Web;

internal class NoteRepository(ILogger<NoteRepository> logger, UserRepository userRepository) {
    private static List<NoteEntity> notes = [];

    public IReadOnlyList<NoteEntity> Notes => notes;

    public bool AddNote(string login, string title, string text) {
        var user = GetUser(login);
        notes.Add(new NoteEntity(Guid.NewGuid(), user.Login, user.Info, DateTime.Now, title, text));
        logger.LogDebug($"User {login} added a note: {title}");
        return true;
    }

    public int DeleteNotesByLogin(string login) => notes.RemoveAll(note => note.OwnerLogin == login);

    public IEnumerable<NoteEntity> GetNotes(string login) {
        GetUser(login);
        foreach (var note in notes) {
            if (note.OwnerLogin == login) {
                yield return note;
            }
        }
    }

    public bool DeleteNote(Guid id) => notes.Remove(notes.FirstOrDefault(note => note.Guid == id));

    public bool ChangeNote(ChangedNoteDTO newNote) {
        logger.LogDebug($"Note change request: {newNote.ToString()}");

        var note = notes.FirstOrDefault(n => n.Guid == newNote.Guid);
        if (note == null) {
            return false;
        }
        note = note with { Creation = note.Creation, Title = newNote.Title, Text = newNote.Text };
        int i = notes.FindIndex(0, note => note.Guid == newNote.Guid);
        notes[i] = note;
        logger.LogDebug("Added note: " + notes.FirstOrDefault(note => note.Guid == newNote.Guid)?.ToString());
        return true;
    }

    public void ChangeOwner(string oldOwner, string newOwner) =>
        notes.ForEach(note => {
            if (note.OwnerLogin == oldOwner) {
                note = note with { OwnerLogin = newOwner };
            }
        });

    private UserEntity GetUser(string login) {
        var user = userRepository.FindByLogin(login);
        if (user == null) {
            var err = $"{login} isn't exist";
            logger.LogDebug(err);
            return UserEntity.Empty;
        }
        return user;
    }
}
