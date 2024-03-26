using Zaza.Web.DataBase.Entities;

namespace Zaza.Web.Stuff.DTO.Response;

internal record class UserBodyResponse(string Username, UserInfo Info) {
    // construct form UserEntity
    public UserBodyResponse(UserEntity entity) : this(entity.Login, entity.Info) { }
    public static explicit operator UserBodyResponse(UserEntity entity) => new UserBodyResponse(entity);
}

