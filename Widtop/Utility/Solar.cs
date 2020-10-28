using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp.Serialization;
using Widtop.Json;

namespace Widtop.Utility
{
    public class Solar
    {
        [JsonObject(MemberSerialization.OptIn)]
        [JsonConverter(typeof(JsonPathConverter))]
        private class SunriseSunsetResponse
        {
            [JsonProperty(PropertyName = "results.sunrise")]
            [JsonConverter(typeof(TimeSpanConverter))]
            public TimeSpan Sunrise { get; set; }

            [JsonProperty(PropertyName = "results.sunset")]
            [JsonConverter(typeof(TimeSpanConverter))]
            public TimeSpan Sunset { get; set; }

            [JsonProperty(PropertyName = "status")]
            public string Status { get; set; }
        }

        private const string Api = "https://api.sunrise-sunset.org";
        private const string Latitude = "57.708870";
        private const string Longitude = "11.974560";

        private readonly Dictionary<DateTime, SunriseSunsetResponse> _cache;

        public Solar()
        {
            _cache = new Dictionary<DateTime, SunriseSunsetResponse>();
        }

        private static IRestSerializer SerializerFactory()
        {
            return new JsonNetSerializer(
                new JsonPathConverter(), 
                new TimeSpanConverter()
            );
        }

        private async Task<SunriseSunsetResponse> Request()
        {
            var client = new RestSharp.RestClient(Api);
            client.UseSerializer(SerializerFactory);

            var request = new RestSharp.RestRequest("json")
                .AddParameter("lat", Latitude)
                .AddParameter("lng", Longitude);

            var response = await client.ExecuteGetAsync<SunriseSunsetResponse>(request);

            if (!response.IsSuccessful || response.Data?.Status != "OK")
            {
                return null;
            }

            _cache[DateTime.UtcNow.Date] = response.Data;
            return response.Data;
        }

        public async Task<TimeSpan?> GetSunrise()
        {
            if (_cache.TryGetValue(DateTime.UtcNow.Date, out var response))
            {
                return response.Sunrise;
            }

            var data = await Request();

            return data?.Sunrise;
        }

        public async Task<TimeSpan?> GetSunset()
        {
            if (_cache.TryGetValue(DateTime.UtcNow.Date, out var response))
            {
                return response.Sunset;
            }

            var data = await Request();

            return data?.Sunset;
        }
    }
}
