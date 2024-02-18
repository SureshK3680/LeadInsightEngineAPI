using Integrate.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace LeadInsightEngineAPI.Models
{
    /// <summary>
    /// This object contains the snapshot of the value of a field at a given time 
    /// </summary>
    [BsonIgnoreExtraElements]
    public class FieldValue
    {
        /// <summary>
        /// Value of the Field
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The source from which this value came
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// The DateTime at which this value was set
        /// </summary>
        public DateTime Timestamp { get; set; }

        public FieldValue()
        {
            // Do nothing
        }

        public FieldValue(string value, string source, DateTime timestamp)
        {
            Value = value;
            Source = source;
            Timestamp = timestamp;
        }

        public static implicit operator string(FieldValue fieldValue)
        {
            return fieldValue.Value;
        }

        public static implicit operator FieldValue(FieldValueModel model)
        {
            var resource = new FieldValue();
            resource.Value = model.Value;
            resource.Timestamp = new DateTime(model.Timestamp);
            resource.Source = model.Source;
            return resource;
        }

        public static implicit operator FieldValueModel(FieldValue resource)
        {
            var model = new FieldValueModel();
            if (!string.IsNullOrEmpty(resource.Value)) model.Value = resource.Value;
            if (!string.IsNullOrEmpty(resource.Source)) model.Source = resource.Source;
            model.Timestamp = resource.Timestamp.Ticks;
            return model;
        }
    }
}
