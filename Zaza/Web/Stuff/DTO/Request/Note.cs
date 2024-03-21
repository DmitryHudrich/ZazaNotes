namespace Zaza.Web.Stuff.DTO.Request;

internal record class NoteDTO(string Title, string Text);
internal record class ChangedNoteDTO(Guid Guid, string Title, string Text);
