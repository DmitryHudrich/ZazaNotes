using Zaza.Web.Stuff;

namespace Zaza.Web.DataBase.Repository;

internal class UserRepository {
    private List<UserEntity> users = [];

    public IReadOnlyList<UserEntity> Users => users;

    public bool Add(UserDTORecieve user) {
        if (users.FirstOrDefault(obj => obj.Login == user.Username) != null) {
            return false;
        }
        users.Add(new UserEntity(Guid.NewGuid(), user.Username, user.Password, TokenService.GenerateRefreshToken(180)));
        return true;
    }

    public void ChangeRefresh(UserEntity user, RefreshToken refresh) {
        var userIndex = users.FindIndex(0, usr => usr.Login == user.Login);
        users[userIndex] = new UserEntity(user.Guid, user.Login, user.Password, refresh);
    }

    public UserEntity? Find(UserDTORecieve dto) =>
        users.FirstOrDefault(user =>
            user.Login == dto.Username &&
            user.Password == dto.Password);

    public UserEntity? Find(string refreshToken) => users.FirstOrDefault(user => user.RefreshToken.Data == refreshToken);
}
