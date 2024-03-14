using MongoDB.Driver;
using Zaza.Web.DataBase;

namespace Zaza.Web;

internal class MongoService {
    public MongoClient Client { get; } = new MongoClient("mongodb://localhost:27017");
    public IMongoDatabase Database => Client.GetDatabase("notes");
    public IMongoCollection<UserEntity> Users => Database.GetCollection<UserEntity>("users");
}
