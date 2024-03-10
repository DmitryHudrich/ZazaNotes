using Zaza.Web.Stuff.DTO.Request;

namespace Zaza.Web.DataBase.Repository;

internal interface IUserRepository {
    IReadOnlyList<UserEntity> Users { get; }

    bool Add(UserMainDTO user);
    bool ChangeInfo(string login, UserInfo newInfo);
    void ChangeRefresh(UserEntity user, RefreshToken refresh);
    bool DeleteByLogin(string login);
    UserEntity? Find(UserMainDTO dto);
    UserEntity? FindByLogin(string login);
    UserEntity? FindByRefresh(string refreshToken);
}

