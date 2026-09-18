using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dnsimple.Services
{
    /// <summary>
    /// Provides a set of methods to extract the data from the JSON payloads
    /// returned by the API calls.
    /// </summary>
    /// <remarks><c>T</c> defines the type of object to be serialized.</remarks>
    public static class JsonTools<T>
    {
        /// <summary>
        /// Deserializes a list of objects of type T from the JSON object passed.
        /// </summary>
        /// <param name="json">The JSON payload</param>
        /// <returns>A list of objects of type T</returns>
        /// <see cref="JToken"/>
        public static List<T> DeserializeList(JToken json)
        {
            return ExtractList(json).Select(item =>
                item.ToObject<T>(Serializer())).OfType<T>().ToList();
        }

        /// <summary>
        /// Deserializes an object of type T from the JSON object passed.
        /// </summary>
        /// <param name="path">The path to the object we want to deserialize in the json payload</param>
        /// <param name="json">The JSON payload</param>
        /// <returns>An object of type T</returns>
        /// <see cref="JToken"/>
        public static T? DeserializeObject(string path, JToken json)
        {
            var token = json.SelectToken(path)
                ?? throw new DnsimpleException($"The response has no '{path}' member.");
            return token.ToObject<T>(Serializer());
        }

        private static JsonSerializer Serializer()
        {
            return new JsonSerializer
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                DateParseHandling = DateParseHandling.DateTimeOffset
            };
        }

        private static IEnumerable<JToken> ExtractList(JToken json)
        {
            var data = json["data"]
                ?? throw new DnsimpleException("The response has no 'data' member.");
            return JArray.FromObject(data).ToList();
        }
    }
}