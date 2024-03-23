namespace Zaza.Web.DataBase.Entities;

internal record class NoteEntity(Guid Id, string Title = "", string Text = "") {
    public DateTime LastChange { get; init; } = DateTime.Now;
};

