using MongoDB.Bson.Serialization.Attributes;

namespace LeadInsightEngineAPI.Models;

[BsonIgnoreExtraElements]
public class EntityRecord
{
    public Guid ContractId { get; set; }
    public Guid Id { get; set; }

    public AllocationKey AllocationKey { get; set; }

    public Guid BatchId { get; set; }

    public DateTime Timestamp { get; set; }
    public List<Field> Fields { get; set; }
}

[BsonIgnoreExtraElements]
public class AllocationKey
{
    public string Key { get; set; }
}