using Integrate.Entities.Models;
using LeadInsightEngineAPI.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace LeadInsightEngineAPI.Repository
{
    public interface IEntitiesRepository
    {
        EntityRecord RetrieveById(Guid id);
    }
    public class EntitiesRepository : IEntitiesRepository
    {
        private readonly string _collectionName;
        private IMongoCollection<EntityRecord> _collection;
        private string connectionString = Constants.Connetionstring;

        public EntitiesRepository()
        {
            _collectionName = Constants.EntitiesCollection;
            RegisterConventions();
            InitializeConnection(connectionString);
        }

        private void InitializeConnection(string connectionString)
        {
            var url = MongoUrl.Create(connectionString);

            var client = new MongoClient(url);
            var database = client.GetDatabase(url.DatabaseName);

            _collection = database.GetCollection<EntityRecord>(_collectionName);
        }

        private void RegisterConventions()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(EntityRecord)))
            {
                BsonClassMap.RegisterClassMap<EntityRecord>(map =>
                {
                    map.AutoMap();
                    map.SetIgnoreExtraElements(true);
                    map.SetIgnoreExtraElementsIsInherited(true);
                });
            }
            if (!BsonClassMap.IsClassMapRegistered(typeof(Resource)))
            {
                BsonClassMap.RegisterClassMap<Resource>(map =>
                {
                    map.AutoMap();
                    var memberMap = map.GetMemberMap(x => x.Id);
                    memberMap.SetIdGenerator(GuidGenerator.Instance);
                    map.SetIdMember(memberMap);
                });
            }
        }

        public EntityRecord RetrieveById(Guid id)
        {
            var idFilter = Builders<EntityRecord>.Filter.Eq(entity => entity.Id, id);
            return _collection.Find(idFilter).FirstOrDefault();
        }
    }
}
