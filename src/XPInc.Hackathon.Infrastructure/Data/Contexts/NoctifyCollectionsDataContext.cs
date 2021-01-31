using MongoDB.Driver;
using XPInc.Hackathon.Core.Domain;
using XPInc.Hackathon.Framework.Data.Contexts;

namespace XPInc.Hackathon.Infrastructure.Data.Contexts
{
    public class NoctifyCollectionsDataContext : MongoDbContext
    {
        public NoctifyCollectionsDataContext(IMongoClient client, IMongoDatabase database) : base(client, database)
        {
            MapCollectionReference<Event>("Noctify.Events");
        }
    }
}
