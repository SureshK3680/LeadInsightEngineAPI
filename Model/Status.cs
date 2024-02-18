using Integrate.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace LeadInsightEngineAPI.Models
{
    //TODO (Jherrera): This shouldn't inherit from StatusResourceModel since it causes casting issues.
    [BsonIgnoreExtraElements]
    public class Status : Models.StatusResourceModel
    {
        public static implicit operator Status(StatusModel model)
        {
            var resource = new Status();
            resource.Key = model.Key;
            resource.Reason = model.Reason;
            resource.Timestamp = new DateTime(model.Timestamp);
            return resource;
        }

        public static implicit operator StatusModel(Status resource)
        {
            var model = new StatusModel();
            model.Timestamp = resource.Timestamp.Ticks;
            if (!string.IsNullOrEmpty(resource.Reason)) model.Reason = resource.Reason;
            if (!string.IsNullOrEmpty(resource.Key)) model.Key = resource.Key;
            return model;
        }
    }
}
