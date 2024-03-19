using MongoDB.Driver;

using Zaza.Web.Stuff.DTO.Request;

namespace Zaza.Web.DataBase.Repository;

internal class UserRepository(ILogger<UserRepository> logger, MongoService mongo) : IUserRepository {
    public async Task<bool> ChangePasswordAsync(string login, string oldPassword, string newPassword) {
        var filter =
            Builders<UserEntity>.Filter.Eq(u => u.Login, login) &
            Builders<UserEntity>.Filter.Eq(u => u.Password.Hash, oldPassword);
        var update = Builders<UserEntity>.Update.Set(u => u.Password.Hash, newPassword);

        var result = await mongo.Users.FindOneAndUpdateAsync(filter, update);
        if (result == null) {
            logger.LogDebug($"User: {login} isn't exist or password is wrong");
            return false;
        }

        return true;
    }

    public async Task<bool> AddAsync(UserMainDTO user) {
        var item = new UserEntity(Guid.NewGuid(), user.Info, user.Login,
                new Password(user.Password), Stuff.TokenService.GenerateRefreshToken(180));
        var filter = Builders<UserEntity>.Filter.Eq(u => u.Login, user.Login);
        var exists = await mongo.Users.FindAsync(filter);
        if (exists.Any()) {
            logger.LogDebug($"User: {user.Login} already exists");
            return false;
        }

        await mongo.Users.InsertOneAsync(item);

        return true;
    }

    public async Task<bool> DeleteByLoginAsync(string login) {
        var filter = Builders<UserEntity>.Filter.Eq(u => u.Login, login);
        var deleteResult = await mongo.Users.DeleteOneAsync(filter);
        return deleteResult.DeletedCount > 0;
    }

    public async Task<bool> ChangeInfoAsync(string login, UserInfo newInfo) {
        var filter =
           Builders<UserEntity>.Filter.Eq(u => u.Login, login);
        var update = Builders<UserEntity>.Update.Set(u => u.Info, newInfo);

        var result = await mongo.Users.FindOneAndUpdateAsync(filter, update);
        if (result == null) {
            logger.LogDebug($"User: {login} isn't exist");
            return false;
        }

        return true;
    }

    public async Task ChangeRefreshAsync(UserEntity user, RefreshToken refresh) {
        var filter =
           Builders<UserEntity>.Filter.Eq(u => u.Login, user.Login);
        var update = Builders<UserEntity>.Update.Set(u => u.RefreshToken, refresh);

        var result = await mongo.Users.FindOneAndUpdateAsync(filter, update);
        if (result == null) {
            logger.LogDebug($"User: {user.Login} isn't exist");
            return;
        }
    }

    public async Task<UserEntity?> FindByFilterAsync(FindFilter filter, string findRequest) {
        UserEntity? res;
        switch (filter) {
            case FindFilter.REFRESH:
                res = await FindByRefreshAsync(findRequest);
                break;
            case FindFilter.LOGIN:
                res = await FindByLoginAsync(findRequest);
                break;
            default:
                res = default;
                break;
        }
        return res;
    }

    public async Task<UserEntity?> FindByLoginAsync(string login) {
        var filter =
            Builders<UserEntity>.Filter.Eq(u => u.Login, login);
        var res = await mongo.Users.FindAsync(filter);
        return res.First();
    }

    public async Task<UserEntity?> FindByRefreshAsync(string refreshToken) {
        var filer =
            Builders<UserEntity>.Filter.Eq(u => u.RefreshToken.Data, refreshToken);
        var res = await mongo.Users.FindAsync(filer);
        return res.First();
    }
}

internal enum FindFilter {
    LOGIN,
    REFRESH,
}

