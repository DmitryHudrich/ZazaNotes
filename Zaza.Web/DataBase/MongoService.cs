using MongoDB.Driver;
using Zaza.Web.DataBase;
using Zaza.Web.Stuff;

namespace Zaza.Web;

internal class MongoService {
    public MongoClient Client { get; } = new MongoClient(Environment.GetEnvironmentVariable(StaticStuff.MongoStingEnvName));
    public IMongoDatabase Database => Client.GetDatabase("notes");
    public IMongoCollection<UserEntity> Users => Database.GetCollection<UserEntity>("users");
}
