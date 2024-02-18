using Integrate.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace LeadInsightEngineAPI.Models
{
    [BsonIgnoreExtraElements]
    public class Field
    {
        public string Key { get; set; }
        public List<FieldValue> FieldValueHistory = new List<FieldValue>();
        public string Type { get; set; }
        public bool IsSystemField { get; set; }
        public string StandardFieldKey { get; set; }

        public Field()
        {
            FieldValueHistory = new List<FieldValue>();
        }

        public Field(string value)
        {
            FieldValueHistory = new List<FieldValue>();
            Set(value, "integrate");
        }

        public Field(string value, string source)
        {
            FieldValueHistory = new List<FieldValue>();
            Set(value, source);
        }


        /// <summary>
        ///     Sets the value of the field with the source as 'integrate'
        /// </summary>
        /// <param name="value">Value to set the Field to</param>
        /// <returns>this</returns>
        public Field Set(string value)
        {
            Set(value, "integrate");
            return this;
        }

        /// <summary>
        ///     Sets the value of the field
        /// </summary>
        /// <param name="value">Value to set the Field to</param>
        /// <param name="source">source from which this value has come</param>
        /// <returns>this</returns>
        public Field Set(string value, string source)
        {
            FieldValueHistory.Add(new FieldValue { Value = value, Timestamp = DateTime.UtcNow, Source = source });
            return this;
        }

        /// <summary>
        ///     Inserts the FieldValue into the history as is
        /// </summary>
        /// <param name="fieldValue"></param>
        /// <returns>this</returns>
        public Field Set(FieldValue fieldValue)
        {
            FieldValueHistory.Add(fieldValue);
            return this;
        }

        /// <summary>
        ///     Gets the most recent FieldValue assigned to this Field
        /// </summary>
        /// <returns>Returns the current value of this Field</returns>
        public FieldValue Get()
        {
            var maxDT = FieldValueHistory.Max(fv => fv.Timestamp);
            return FieldValueHistory.FirstOrDefault(fv => fv.Timestamp == maxDT);

            // this is some voodoo I found online since .Max only returns the max value, not the object in which the value is contained
            //return _history.Aggregate((agg, next) => next.Timestamp > agg.Timestamp ? next : agg).Value; 
        }

        /// <summary>
        ///     Gets the most recent FieldValue assigned to this Field from the given source
        /// </summary>
        /// <param name="source">Source of the value</param>
        /// <returns>Returns the given FieldValue</returns>
        public FieldValue Get(string source)
        {
            var maxDT = FieldValueHistory.Max(fv => fv.Timestamp);
            return FieldValueHistory.FirstOrDefault(fv => fv.Timestamp == maxDT && fv.Source == source);

            // this is some voodoo I found online since .Max only returns the max value, not the object in which the value is contained
            //return _history.Aggregate((agg, next) => next.Timestamp > agg.Timestamp && next.Source == source  ? next : agg).Value;
        }

        /// <summary>
        ///     Gets the most recent FieldValue assigned to this field from the given source at or before the given time
        /// </summary>
        /// <param name="source">Source of the value</param>
        /// <param name="timestamp">The highest desired date time for the field value</param>
        /// <returns>Returns the given FieldValue</returns>
        public FieldValue Get(string source, DateTime timestamp)
        {
            var maxDT = FieldValueHistory.Where(fv => fv.Timestamp <= timestamp).Max(fv => fv.Timestamp);
            return FieldValueHistory.FirstOrDefault(fv => fv.Timestamp == maxDT && fv.Source == source);
        }

        public static implicit operator string(Field field)
        {
            return field.Get().Value;
        }

        public static implicit operator FieldValue(Field field)
        {
            return field.Get();
        }

        public static implicit operator Field(FieldModel model)
        {
            var resource = new Field();
            resource.Key = model.Key;
            resource.Type = model.Type;
            resource.FieldValueHistory = model.FieldValueHistory.Select(p => (FieldValue)p).ToList();
            resource.StandardFieldKey = model.StandardFieldKey;
            return resource;
        }

        public static implicit operator FieldModel(Field resource)
        {
            var model = new FieldModel();
            if (!string.IsNullOrEmpty(resource.Key)) model.Key = resource.Key;
            if (!string.IsNullOrEmpty(resource.Type)) model.Type = resource.Type;
            if (resource.FieldValueHistory != null && resource.FieldValueHistory.Any())
            {
                model.FieldValueHistory.Add(resource.FieldValueHistory.Select(p => (FieldValueModel)p));
            }
            return model;
        }
    }
}
