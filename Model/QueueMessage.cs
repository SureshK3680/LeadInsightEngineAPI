using MongoDB.Bson.Serialization.Attributes;

namespace LeadInsightEngineAPI.Models
{
    [BsonIgnoreExtraElements]
    public class QueueMessage
    {
        public Guid BatchId { get; set; }

        public Guid ContractId { get; set; }

        public Guid OwnerId { get; set; }
    }
}
