using Zaza.Web.DataBase;
using Zaza.Web.Stuff.DTO.Request;

namespace Zaza.Web;

internal interface INoteRepository {
    Task<bool> AddNoteAsync(string login, string title, string text);
    Task<bool> ChangeNoteAsync(ChangedNoteDTO newNote);
    Task<bool> DeleteNoteAsync(Guid id);
    Task<int> DeleteNotesByLoginAsync(string login);
    IAsyncEnumerable<NoteEntity> GetNotesAsync(string login);
}

