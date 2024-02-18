using MongoDB.Bson.Serialization.Attributes;

namespace LeadInsightEngineAPI.Models;

[BsonIgnoreExtraElements]
public class AllocationRecord
{
    public Guid Id { get; set; }
    public Guid ContractId { get; set; }

    public bool BatchLock { get; set; }
    public int InProgress { get; set; }
}