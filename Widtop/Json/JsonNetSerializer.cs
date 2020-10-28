using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serialization;

namespace Widtop.Json
{
    public class JsonNetSerializer : IRestSerializer
    {
        public JsonConverter[] Converters { get; }

        public string ContentType { get; set; } = "application/json";

        public DataFormat DataFormat { get; } = DataFormat.Json;

        public string[] SupportedContentTypes { get; } =
        {
            "application/json", "text/json", "text/x-json", "text/javascript", "*+json"
        };

        public JsonNetSerializer(params JsonConverter[] converters)
        {
            Converters = converters;
        }

        public string Serialize(object obj) => 
            JsonConvert.SerializeObject(obj, Converters);

#pragma warning disable 618
        public string Serialize(Parameter bodyParameter) =>
#pragma warning restore 618
            JsonConvert.SerializeObject(bodyParameter.Value, Converters);

        public T Deserialize<T>(IRestResponse response) =>
            JsonConvert.DeserializeObject<T>(response.Content, Converters);
    }
}