using Zaza.Web.DataBase;
using Zaza.Web.Stuff.DTO.Request;

namespace Zaza.Web;

internal interface INoteRepository {
    IReadOnlyList<NoteEntity> Notes { get; }
    bool AddNote(string login, string title, string text);
    bool ChangeNote(ChangedNoteDTO newNote);
    bool DeleteNote(Guid id);
    int DeleteNotesByLogin(string login);
    IEnumerable<NoteEntity> GetNotes(string login);
}

