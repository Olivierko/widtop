using System;
using System.Globalization;
using Newtonsoft.Json;

namespace Widtop.Json
{
    public class TimeSpanConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TimeSpan);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var val = (string)reader.Value;

            return DateTime.TryParse(val, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date) 
                ? date.TimeOfDay 
                : TimeSpan.Zero;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((TimeSpan)value).ToString("hh:mm:ss tt"));
        }
    }
}