using Integrate.Entities.Models;
using LeadInsightEngineAPI.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace LeadInsightEngineAPI.Repository
{
    public interface IEntitiesBatchRepository
    {
        void Insert(BatchRecord batchrecords);
    }
    public class EntitiesBatchRepository : IEntitiesBatchRepository
    {
        private readonly string _collectionName;
        private IMongoCollection<BatchRecord> _collection;
        private string connectionString = Constants.Connetionstring;

        public EntitiesBatchRepository()
        {
            _collectionName = Constants.EntityBatchCollection;
            RegisterConventions();
            InitializeConnection(connectionString);
        }

        private void InitializeConnection(string connectionString)
        {
            var url = MongoUrl.Create(connectionString);

            var client = new MongoClient(url);
            var database = client.GetDatabase(url.DatabaseName);

            _collection = database.GetCollection<BatchRecord>(_collectionName);
        }

        private void RegisterConventions()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(BatchRecord)))
            {
                BsonClassMap.RegisterClassMap<BatchRecord>(map =>
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

        public void Insert(BatchRecord batchrecords)
        {
            _collection.InsertOne(batchrecords);
        }
    }
}
