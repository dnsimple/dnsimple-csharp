using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace dnsimple
{
    public static class DataTools
    {
        public static IEnumerable<JToken> ExtractList(JToken json)
        {
            return JArray.FromObject(json["data"]).ToList();
        }
    }
}