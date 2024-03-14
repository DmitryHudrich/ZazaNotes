using MongoDB.Driver;
using Zaza.Web.DataBase;
using Zaza.Web.DataBase.Repository;
using Zaza.Web.Stuff.DTO.Request;

namespace Zaza.Web;

internal sealed class NoteRepository(ILogger<NoteRepository> logger, IUserRepository userRepository, MongoService mongo) : INoteRepository {
    public async Task<bool> AddNoteAsync(string login, string title, string text) {
        var filter = Builders<UserEntity>.Filter.Eq(u => u.Login, login);
        var update = Builders<UserEntity>.Update.Push(u => u.Notes, new NoteEntity(Guid.NewGuid(), title, text));
        var updateResult = await mongo.Users.UpdateOneAsync(filter, update);

        return updateResult.ModifiedCount > 0;
    }

    public async Task<List<NoteEntity>> GetNotesAsync(string login) {
        var user = await GetUserAsync(login);
        if (user == null) {
            return [];
        }
        return user.Notes;
    }

    public async Task<bool> DeleteNoteAsync(Guid id) {
        var filter = Builders<UserEntity>.Filter.ElemMatch(u => u.Notes, Builders<NoteEntity>.Filter.Eq(n => n.Id, id));
        var update = Builders<UserEntity>.Update.PullFilter(u => u.Notes, Builders<NoteEntity>.Filter.Eq(n => n.Id, id));

        var res = await mongo.Users.FindOneAndUpdateAsync(filter, update);
        return res != null;
    }

    public async Task<bool> ChangeNoteAsync(ChangedNoteDTO newNote, string login) {
        // Фильтр для нахождения пользователя по логину и заметки по GUID
        var filter = Builders<UserEntity>.Filter.And(
            Builders<UserEntity>.Filter.Eq(u => u.Login, login),
            Builders<UserEntity>.Filter.ElemMatch(u => u.Notes, Builders<NoteEntity>.Filter.Eq(n => n.Id, newNote.Guid))
        );

        // Обновление для изменения заголовка и текста заметки
        var update = Builders<UserEntity>.Update.Set("Notes.$", new NoteEntity(newNote.Guid, newNote.Title, newNote.Text));

        // Выполняем запрос к базе данных MongoDB
        var result = await mongo.Users.FindOneAndUpdateAsync(filter, update);

        // Возвращаем результат операции
        return result != null;
    }

    private async Task<UserEntity?> GetUserAsync(string login) {
        var res = userRepository.FindByLoginAsync(login).Result;
        return res;
    }
}
