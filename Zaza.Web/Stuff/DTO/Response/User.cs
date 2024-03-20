using Zaza.Web.DataBase.Entities;

namespace Zaza.Web.Stuff.DTO.Response;

internal record class UserBodyResponse(string Username, UserInfo info);

