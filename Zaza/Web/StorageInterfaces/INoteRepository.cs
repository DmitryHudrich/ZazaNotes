using Zaza.Web.DataBase.Entities;
using Zaza.Web.Stuff.DTO.Request;

namespace Zaza.Web.StorageInterfaces;

internal interface INoteRepository {
    Task<bool> AddNoteAsync(Guid id, string title, string text);
    Task<bool> ChangeNoteAsync(ChangedNoteDTO newNote, Guid userId);
    Task<bool> DeleteNoteAsync(Guid noteId);
    Task<List<NoteEntity>> GetNotesAsync(Guid id);
}
