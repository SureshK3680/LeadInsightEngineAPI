using Integrate.Entities.Models;
using LeadInsightEngineAPI.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Driver;

namespace LeadInsightEngineAPI.Repository
{
    public interface IAllocationrepository
    {
        void FindOneAndUpdate(Guid contractId);
    }
    public class Allocationrepository : IAllocationrepository
    {
        private readonly string _collectionName;
        private IMongoCollection<AllocationRecord> _collection;
        private string connectionString = Constants.Connetionstring;

        public Allocationrepository()
        {
            _collectionName = Constants.AllocationCollection;
            RegisterConventions();
            InitializeConnection(connectionString);
        }

        private void InitializeConnection(string connectionString)
        {
            var url = MongoUrl.Create(connectionString);

            var client = new MongoClient(url);
            var database = client.GetDatabase(url.DatabaseName);

            _collection = database.GetCollection<AllocationRecord>(_collectionName);
        }

        private void RegisterConventions()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(AllocationRecord)))
            {
                BsonClassMap.RegisterClassMap<AllocationRecord>(map =>
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

        public void FindOneAndUpdate(Guid contractId)
        {
            var filterobj = Builders<AllocationRecord>.Filter.Where(p => p.ContractId == contractId);
            var updateobj = Builders<AllocationRecord>.Update.Set(p => p.InProgress, 0).Set(p => p.BatchLock, true);
            _collection.FindOneAndUpdate(filterobj, updateobj);
        }
    }
}
