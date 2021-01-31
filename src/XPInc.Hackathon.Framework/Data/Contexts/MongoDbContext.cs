using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace XPInc.Hackathon.Framework.Data.Contexts
{
    public class MongoDbContext : IMongoDbContext
    {
        public IMongoClient Client { get; }
        public IMongoDatabase Database { get; }

        private static readonly Dictionary<Type, string> _collectionReferences = new Dictionary<Type, string>();

        protected MongoDbContext(IMongoClient client, IMongoDatabase database)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            Database = database ?? throw new ArgumentNullException(nameof(database));
        }

        public static void MapCollectionReference<T>(string collectionName)
        {
            MapCollectionReference(typeof(T), collectionName);
        }

        public static void MapCollectionReference(Type referenceType, string collectionName)
        {
            if (referenceType is null)
            {
                throw new ArgumentNullException(nameof(referenceType));
            }

            if (string.IsNullOrWhiteSpace(collectionName))
            {
                throw new ArgumentException("Invalid name.", nameof(collectionName));
            }

            if (!_collectionReferences.ContainsKey(referenceType))
            {
                _collectionReferences.Add(referenceType, collectionName);
            }
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            if (!_collectionReferences.ContainsKey(typeof(T)))
            {
                MapCollectionReference<T>(name);
            }

            return GetCollection<T>();
        }

        public IMongoCollection<T> GetCollection<T>()
        {
            var referenceType = typeof(T);

            if (!_collectionReferences.ContainsKey(referenceType))
            {
                throw new ArgumentException($"No collection found for type \"{referenceType.Name}\". Use \"{nameof(MapCollectionReference)}\" to define this reference.");
            }

            var collectionName = _collectionReferences[typeof(T)];
            return Database.GetCollection<T>(collectionName);
        }
    }
}
