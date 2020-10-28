using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Widtop.Json
{
    public class JsonPathConverter : JsonConverter
    {
        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer
        )
        {
            var jo = JObject.Load(reader);
            var target = Activator.CreateInstance(objectType);

            foreach (var prop in objectType.GetProperties().Where(p => p.CanRead && p.CanWrite))
            {
                var propertyAttribute = prop.GetCustomAttributes(true)
                    .OfType<JsonPropertyAttribute>()
                    .FirstOrDefault();

                var path = propertyAttribute != null ? propertyAttribute.PropertyName : prop.Name;

                if (serializer.ContractResolver is DefaultContractResolver resolver)
                {
                    path = resolver.GetResolvedPropertyName(path);
                }

                if (!Regex.IsMatch(path, @"^[a-zA-Z0-9_.-]+$"))
                {
                    throw new InvalidOperationException($"JProperties of JsonPathConverter can have only letters, numbers, underscores, hyphens and dots but name was ${path}.");
                }

                var token = jo.SelectToken(path);

                if (token != null && token.Type != JTokenType.Null)
                {
                    var value = token.ToObject(prop.PropertyType, serializer);
                    prop.SetValue(target, value, null);
                }
            }

            return target;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.GetCustomAttributes(true).OfType<JsonPathConverter>().Any();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var properties = value.GetType().GetRuntimeProperties().Where(p => p.CanRead && p.CanWrite);
            var main = new JObject();

            foreach (var prop in properties)
            {
                var propertyAttribute = prop.GetCustomAttributes(true)
                    .OfType<JsonPropertyAttribute>()
                    .FirstOrDefault();

                var path = propertyAttribute != null ? propertyAttribute.PropertyName : prop.Name;

                if (serializer.ContractResolver is DefaultContractResolver resolver)
                {
                    path = resolver.GetResolvedPropertyName(path);
                }

                var nesting = path.Split('.');
                var lastLevel = main;

                for (var i = 0; i < nesting.Length; i++)
                {
                    if (i == nesting.Length - 1)
                    {
                        lastLevel[nesting[i]] = new JValue(prop.GetValue(value));
                    }
                    else
                    {
                        if (lastLevel[nesting[i]] == null)
                        {
                            lastLevel[nesting[i]] = new JObject();
                        }

                        lastLevel = (JObject)lastLevel[nesting[i]];
                    }
                }
            }

            serializer.Serialize(writer, main);
        }
    }
}