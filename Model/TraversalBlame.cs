using Integrate.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace LeadInsightEngineAPI.Models
{
    [BsonIgnoreExtraElements]
    public class TraversalBlame
    {
        [BsonIgnoreIfNull]
        public string ApiKeyId { get; set; }

        public static explicit operator TraversalBlameModel(TraversalBlame source)
        {
            if (source == null)
            {
                return null;
            }
            return new TraversalBlameModel
            {
                ApiKeyId = source.ApiKeyId ?? ""
            };
        }

        public static explicit operator TraversalBlame(TraversalBlameModel source)
        {
            if (source == null)
            {
                return null;
            }
            return new TraversalBlame
            {
                ApiKeyId = source.ApiKeyId switch
                {
                    "" => null,
                    var value => value
                }
            };
        }
    }
}
