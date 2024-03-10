using Zaza.Web.DataBase;

namespace Zaza.Web.Stuff.DTO.Request;

internal record class UserMainDTO(string Login, string Name, UserInfo Info, string Password = "");
internal record class UserLoginRequestDTO(string Login, string Password);
