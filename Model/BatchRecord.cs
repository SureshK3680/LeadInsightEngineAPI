using MongoDB.Bson.Serialization.Attributes;

namespace LeadInsightEngineAPI.Models;

[BsonIgnoreExtraElements]
public class BatchRecord
{
    public Guid Id { get; set; }

    public Guid OwnerId { get; set; }

    public Guid ContractId { get; set; }

    public Guid BatchId { get; set; }

    public Guid EntityId { get; set; }

    public string AllocationKey { get; set; }

    public object[] Parents { get; set; }

    public object Attributes { get; set; }

    public DateTime Timestamp { get; set; }

    public Disposition Disposition { get; set; }
    public Dictionary<string, FieldValue> FieldValues { get; set; }
}

[BsonIgnoreExtraElements]
public class Disposition
{
    public string Reason { get; set; }

    public string Code { get; set; }
}