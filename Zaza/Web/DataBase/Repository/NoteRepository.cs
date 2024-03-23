using MongoDB.Driver;
using Zaza.Web.DataBase.Entities;
using Zaza.Web.StorageInterfaces;
using Zaza.Web.Stuff.DTO.Request;

namespace Zaza.Web.DataBase.Repository;

internal sealed class NoteRepository(IUserRepository userRepository, MongoService mongo) : INoteRepository {
    public async Task<bool> AddNoteAsync(Guid id, string title, string text) {
        var filter = Builders<UserEntity>.Filter.Eq(u => u.Id, id);
        var update = Builders<UserEntity>.Update.Push(u => u.Notes, new NoteEntity(Guid.NewGuid(), title, text));
        var updateResult = await mongo.Users.UpdateOneAsync(filter, update);

        return updateResult.ModifiedCount > 0;
    }

    public async Task<List<NoteEntity>> GetNotesAsync(Guid userId) {
        var user = await GetUserAsync(userId);
        return user == null ? [] : user.Notes;
    }

    public async Task<bool> DeleteNoteAsync(Guid id) {
        var filter = Builders<UserEntity>.Filter.ElemMatch(u => u.Notes, Builders<NoteEntity>.Filter.Eq(n => n.Id, id));
        var update = Builders<UserEntity>.Update.PullFilter(u => u.Notes, Builders<NoteEntity>.Filter.Eq(n => n.Id, id));

        var res = await mongo.Users.FindOneAndUpdateAsync(filter, update);
        return res != null;
    }

    public async Task<bool> ChangeNoteAsync(ChangedNoteDTO newNote, Guid id) {
        var filter = Builders<UserEntity>.Filter.And(
            Builders<UserEntity>.Filter.Eq(u => u.Id, id),
            Builders<UserEntity>.Filter.ElemMatch(u => u.Notes, Builders<NoteEntity>.Filter.Eq(n => n.Id, newNote.Guid))
        );

        var update = Builders<UserEntity>.Update.Set("Notes.$", new NoteEntity(newNote.Guid, newNote.Title, newNote.Text));

        var result = await mongo.Users.FindOneAndUpdateAsync(filter, update);

        return result != null;
    }

    private async Task<UserEntity?> GetUserAsync(Guid id) {
        var res = await userRepository.FindByFilterAsync(FindFilter.ID, id);
        return res;
    }
}
