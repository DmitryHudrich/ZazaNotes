namespace Zaza.Web.DataBase.Entities;

internal record class UserInfo(
        string FirstName, string LastName = "", string Description = "", string Country = "", string Photo = "");

