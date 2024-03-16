using Zaza.Web.Stuff.DTO.Request;

namespace Zaza.Web.DataBase.Repository;

internal interface IUserRepository {
    Task<bool> ChangePasswordAsync(string login, string oldPassword, string newPassword);
    Task<bool> AddAsync(UserMainDTO user);
    Task<bool> ChangeInfoAsync(string login, UserInfo newInfo);
    Task ChangeRefreshAsync(UserEntity user, RefreshToken refresh);
    Task<bool> DeleteByLoginAsync(string login);
    Task<UserEntity?> FindByLoginAsync(string login);
    Task<UserEntity?> FindByRefreshAsync(string refreshToken);
}

