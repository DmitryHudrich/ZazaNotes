using Zaza.Web.Stuff.DTO.Request;

namespace Zaza.Web.DataBase.Repository;

internal class UserRepository(ILogger<UserRepository> logger) : IUserRepository {
    private static List<UserEntity> users = [];

    public IReadOnlyList<UserEntity> Users => users;

    public bool ChangePassword(string login, string oldPassword, string newPassword) {
        var i = users.FindIndex(0, user => user.Login == login);
        var user = users[i];

        if (user.Password != oldPassword) {
            logger.LogDebug($"User {login}: password is incorrect or user not found.");
            return false;
        }

        users[i] = user with { Password = newPassword };

        return true;
    }


    public bool Add(UserMainDTO user) {
        if (users.FirstOrDefault(obj => obj.Login == user.Login) != null) {
            logger.LogDebug($"User: {user.Login} isn't exist");
            return false;
        }
        users.Add(new UserEntity(Guid.NewGuid(), user.Info, user.Login, user.Password, Stuff.TokenService.GenerateRefreshToken(180)));
        logger.LogDebug($"User {user.Login} was added to database");
        logger.LogTrace("Users count: " + users.Count + " Last user: " + users.Last().Login);
        return true;
    }

    public bool DeleteByLogin(string login) {
        var user = FindByLogin(login);
        if (user != null) {
            users.Remove(user);
            return true;
        }
        return false;
    }

    public bool ChangeInfo(string login, UserInfo newInfo) {
        var user = FindByLogin(login);
        if (user == null) {
            return false;
        }
        users.Remove(user);
        user = user with { Info = newInfo };
        users.Add(user);
        return true;
    }

    public void ChangeRefresh(UserEntity user, RefreshToken refresh) {
        logger.LogDebug($"{user.Login}: RefreshToken was refreshed:");
        var userIndex = users.FindIndex(0, usr => usr.Login == user.Login);
        users[userIndex] = new UserEntity(user.Guid, user.Info, user.Login, user.Password, refresh);
    }

    public UserEntity? Find(UserMainDTO dto) =>
        users.FirstOrDefault(user =>
            user.Login == dto.Login &&
            user.Password == dto.Password);

    public UserEntity? FindByLogin(string login) => users.FirstOrDefault(user => user.Login == login);


    public UserEntity? FindByRefresh(string refreshToken) => users.FirstOrDefault(user => user.RefreshToken.Data == refreshToken);
}
