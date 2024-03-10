using Zaza.Web.DataBase;
using Zaza.Web.DataBase.Repository;
using Zaza.Web.Stuff;

namespace Zaza.Web;

internal class NoteRepository(ILogger<NoteRepository> logger, UserRepository userRepository) {
    private static List<NoteEntity> notes = [];

    public IReadOnlyList<NoteEntity> Notes => notes;

    public bool AddNote(string login, string title, string text) {
        var user = GetUser(login);
        notes.Add(new NoteEntity(Guid.Empty, user.Login, user.Info, DateTime.Now, title, text));
        logger.LogDebug($"User {login} added a note: {title}");
        return true;
    }

    public IEnumerable<NoteEntity> GetNotes(string login) {
        GetUser(login);
        foreach (var note in notes) {
            if (note.OwnerLogin == login) {
                yield return note;
            }
        }
    }

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
