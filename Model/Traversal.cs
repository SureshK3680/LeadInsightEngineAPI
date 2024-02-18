using Integrate.Entities;
using Integrate.Protobuf;
using MongoDB.Bson.Serialization.Attributes;

namespace LeadInsightEngineAPI.Models
{
    [BsonIgnoreExtraElements]
    public class Traversal
    {
        public Guid Id { get; set; }
        public Guid BatchId { get; set; }
        public string Ingress { get; set; }
        public DateTime Timestamp { get; set; }
        public List<Status> Statuses { get; set; }

        public TraversalBlame Blame { get; set; }

        public Traversal()
        {
            Id = Guid.NewGuid();
            Ingress = TraversalIngress.Unspecified;
            Timestamp = DateTime.UtcNow;
            Statuses = new List<Status>();
        }

        public static implicit operator TraversalModel(Traversal source)
        {
            var ingress = string.IsNullOrEmpty(source.Ingress)
                ? TraversalIngress.Unspecified : source.Ingress;
            var timestamp = source.Timestamp.Equals(default)
                ? DateTime.UtcNow.Ticks : source.Timestamp.Ticks;
            var target = new TraversalModel
            {
                Id = source.Id.ToByteString(),
                BatchId = source.BatchId.ToByteString(),
                Ingress = ingress,
                Timestamp = timestamp,
                Blame = (TraversalBlameModel)source.Blame
            };

            source.Statuses.ForEach(status => target.Statuses.Add(status));

            return target;
        }

        public static implicit operator Traversal(TraversalModel source)
        {
            var ingress = string.IsNullOrEmpty(source.Ingress)
                ? TraversalIngress.Unspecified : source.Ingress;
            var timestamp = source.Timestamp.Equals(default(DateTime).Ticks)
                ? DateTime.UtcNow : new DateTime(source.Timestamp);
            var target = new Traversal
            {
                Id = source.Id.ToGuid(),
                BatchId = source.BatchId.ToGuid(),
                Ingress = ingress,
                Timestamp = timestamp,
                Blame = (TraversalBlame)source.Blame
            };

            source.Statuses.ToList().ForEach(status => target.Statuses.Add(status));

            return target;
        }
    }
}
