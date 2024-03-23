namespace Zaza.Web.DataBase.Entities;

internal record class RefreshToken(string Data, TimeSpan Expire);
