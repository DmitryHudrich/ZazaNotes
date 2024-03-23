using Zaza.Web.DataBase.Entities;
using Zaza.Web.DataBase.Repository;
using Zaza.Web.Stuff.DTO.Request;

namespace Zaza.Web.StorageInterfaces;

internal interface IUserRepository {
    Task ChangeTelegramId(Guid userId, ulong telegramId);
    Task<bool> ChangePasswordAsync(string login, string oldPassword, string newPassword);
    Task<bool> AddAsync(UserMainDTO user);
    Task<bool> AddAsync(UserTelegramDTO user);
    Task<bool> ChangeInfoAsync(Guid id, UserInfo newInfo);
    Task ChangeRefreshAsync(UserEntity user, RefreshToken refresh);
    Task<bool> DeleteByIdAsync(Guid id);
    Task<UserEntity?> FindByFilterAsync<T>(FindFilter filter, T findRequest);
}
