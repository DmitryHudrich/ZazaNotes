using MongoDB.Driver;
using Zaza.Web.DataBase.Entities;
using Zaza.Web.StorageInterfaces;
using Zaza.Web.Stuff.DTO.Request;
using Zaza.Web.Stuff.StaticServices;

namespace Zaza.Web.DataBase.Repository;

internal sealed class UserRepository(ILogger<UserRepository> logger, MongoService mongo) : IUserRepository {
    public async Task<bool> ChangePasswordAsync(string login, string oldPassword, string newPassword) {
        var filter =
            Builders<UserEntity>.Filter.Eq(u => u.Login, login) &
            Builders<UserEntity>.Filter.Eq(u => u.Password.Hash, HashHelper.Hash(oldPassword));
        var update = Builders<UserEntity>.Update.Set(u => u.Password.Hash, HashHelper.Hash(newPassword));

        var result = await mongo.Users.FindOneAndUpdateAsync(filter, update);
        if (result == null) {
            logger.LogDebug($"User: {login} isn't exist or password is wrong");
            return false;
        }

        return true;
    }

    public async Task<bool> AddAsync(UserTelegramDTO dto) => await HandleUserRegistration(place: RegistrationPlace.TELEGRAM, telegramDTO: dto);

    public async Task<bool> AddAsync(UserMainDTO user) => await HandleUserRegistration(place: RegistrationPlace.FROM_API, apiDTO: user);

    private async Task<bool> HandleUserRegistration(RegistrationPlace place, UserMainDTO? apiDTO = default, UserTelegramDTO? telegramDTO = default) {
        var filter = place switch {
            RegistrationPlace.TELEGRAM => Builders<UserEntity>.Filter.Eq(u => u.TelegramId, telegramDTO!.Id),
            RegistrationPlace.FROM_API => Builders<UserEntity>.Filter.Eq(u => u.Login, apiDTO!.Login),
            RegistrationPlace.UNKNOWN => default,
            _ => default,
        };
        var user = place switch {
            RegistrationPlace.TELEGRAM =>
                telegramDTO != null
                ? new UserEntity(
                     id: Guid.NewGuid(),
                     info: new UserInfo(
                        FirstName: telegramDTO.FirstName,
                        LastName: telegramDTO.LastName,
                        Photo: telegramDTO.Photo),
                     telegramId: telegramDTO.Id) {
                    AuthInfo = new AuthInfo {
                        Telegram = true
                    }
                } :
                throw new ArgumentNullException(nameof(telegramDTO)),
            RegistrationPlace.FROM_API =>
                apiDTO != null
                ? new UserEntity(
                    id: Guid.NewGuid(),
                    info: apiDTO.Info,
                    login: apiDTO.Login,
                    password: apiDTO.Password,
                    refresh: Stuff.TokenService.GenerateRefreshToken(180)) {
                    AuthInfo = new AuthInfo {
                        Web = true
                    }
                } :
                throw new ArgumentNullException(nameof(apiDTO)),
            RegistrationPlace.UNKNOWN => default,
            _ => default,
        };

        return user != null && await AddToDb(user, filter);
    }

    private async Task<bool> AddToDb(UserEntity user, FilterDefinition<UserEntity>? filter) {
        var item = user;
        var exists = await mongo.Users.FindAsync(filter);

        if (exists.Any()) {
            logger.LogDebug($"User: {user.Login} already exists");
            return false;
        }
        System.Console.WriteLine(user);
        await mongo.Users.InsertOneAsync(item);

        return true;

    }

    public async Task<bool> DeleteByIdAsync(Guid id) {
        var filter = Builders<UserEntity>.Filter.Eq(u => u.Id, id);
        var deleteResult = await mongo.Users.DeleteOneAsync(filter);
        return deleteResult.DeletedCount > 0;
    }

    public async Task ChangeTelegramId(Guid userId, ulong telegramId) {
        var filter =
           Builders<UserEntity>.Filter.Eq(u => u.Id, userId);
        var update = Builders<UserEntity>.Update.Set(u => u.TelegramId, telegramId);

        var result = await mongo.Users.FindOneAndUpdateAsync(filter, update);
        if (result == null) {
            logger.LogDebug($"User: {userId} isn't exist");
        }
    }

    public async Task<bool> ChangeInfoAsync(Guid id, UserInfo newInfo) {
        var filter =
           Builders<UserEntity>.Filter.Eq(u => u.Id, id);
        var update = Builders<UserEntity>.Update.Set(u => u.Info, newInfo);

        var result = await mongo.Users.FindOneAndUpdateAsync(filter, update);
        if (result == null) {
            logger.LogDebug($"User: {id} isn't exist");
            return false;
        }

        return true;
    }

    public async Task ChangeRefreshAsync(UserEntity user, RefreshToken refresh) {
        var filter =
           Builders<UserEntity>.Filter.Eq(u => u.Id, user.Id);
        var update = Builders<UserEntity>.Update.Set(u => u.Refresh, refresh);

        var result = await mongo.Users.FindOneAndUpdateAsync(filter, update);
        if (result == null) {
            logger.LogDebug($"User: {user.Login} isn't exist");
            return;
        }
    }

    public async Task<UserEntity?> FindByFilterAsync<T>(FindFilter filter, T findRequest) {
        var res = filter switch {
            FindFilter.REFRESH => findRequest is string s ?
                await FindByRefreshAsync(s) : throw new ArgumentException("Wrong type", nameof(findRequest)),
            FindFilter.LOGIN => findRequest is string s ?
                await FindByLoginAsync(s) : throw new ArgumentException("Wrong type", nameof(findRequest)),
            FindFilter.TELEGRAM_ID => findRequest is ulong l ?
                await FindByTelegramIdAsync(l) : throw new ArgumentException("Wrong type", nameof(findRequest)),
            FindFilter.ID => findRequest is Guid g ?
                await FindByIdAsync(g) : throw new ArgumentException("Wrong type", nameof(findRequest)),
            _ => default
        };
        return res;
    }

    private async Task<UserEntity?> FindByIdAsync(Guid id) {
        var filter =
            Builders<UserEntity>.Filter.Eq(u => u.Id, id);
        var res = await mongo.Users.FindAsync(filter);
        return res.FirstOrDefault();
    }

    private async Task<UserEntity?> FindByTelegramIdAsync(ulong id) {
        var filter =
            Builders<UserEntity>.Filter.Eq(u => u.TelegramId, id);
        var res = await mongo.Users.FindAsync(filter);
        return res.FirstOrDefault();
    }

    private async Task<UserEntity?> FindByLoginAsync(string login) {
        var filter =
            Builders<UserEntity>.Filter.Eq(u => u.Login, login);
        var res = await mongo.Users.FindAsync(filter);
        return res.First();
    }

    private async Task<UserEntity?> FindByRefreshAsync(string refreshToken) {
        var filer =
            Builders<UserEntity>.Filter.Eq(u => u.Refresh.Data, refreshToken);
        var res = await mongo.Users.FindAsync(filer);
        return res.FirstOrDefault();
    }
}

internal enum FindFilter {
    LOGIN,
    REFRESH,
    ID,
    TELEGRAM_ID,
}

