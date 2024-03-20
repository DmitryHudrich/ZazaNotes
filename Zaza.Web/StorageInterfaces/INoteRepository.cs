using Zaza.Web.DataBase.Entities;
using Zaza.Web.Stuff.DTO.Request;

namespace Zaza.Web;

internal interface INoteRepository {
    Task<bool> AddNoteAsync(string login, string title, string text);
    Task<bool> ChangeNoteAsync(ChangedNoteDTO newNote, string login);
    Task<bool> DeleteNoteAsync(Guid id);
    Task<List<NoteEntity>> GetNotesAsync(string login);
}
