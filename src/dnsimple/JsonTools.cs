using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

//TODO: Look into the suggestions... ;)
namespace dnsimple
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
                item.ToObject<T>(Serializer())).ToList();
        }

        /// <summary>
        /// Deserializes an object of type T from the JSON object passed.
        /// </summary>
        /// <param name="path">The path to the object we want to deserialize in the json payload</param>
        /// <param name="json">The JSON payload</param>
        /// <returns>An object of type T</returns>
        /// <see cref="JToken"/>
        public static T DeserializeObject(string path, JToken json)
        {
            return json.SelectToken(path).ToObject<T>(Serializer());
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
            return JArray.FromObject(json["data"]).ToList();
        }
    }
}