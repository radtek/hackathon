using MongoDB.Driver;

namespace XPInc.Hackathon.Framework.Data.Contexts
{
    public interface IMongoDbContext : IDataContext
    {
        IMongoCollection<T> GetCollection<T>(string name);
        IMongoCollection<T> GetCollection<T>();
    }
}
