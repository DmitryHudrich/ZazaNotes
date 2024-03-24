using MongoDB.Driver;

using Zaza.Web.DataBase.Entities;
using Zaza.Web.Stuff;

namespace Zaza.Web.DataBase;

internal class MongoService {
    public MongoClient Client { get; } = new MongoClient(Environment.GetEnvironmentVariable(StaticStuff.MongoStringEnvName));
    public IMongoDatabase Database => Client.GetDatabase("notes");
    public IMongoCollection<UserEntity> Users => Database.GetCollection<UserEntity>("users");
}
