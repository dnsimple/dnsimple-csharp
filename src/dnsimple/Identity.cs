using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace dnsimple
{
    public class IdentityService
    {
        private IClient Client { get; }

        public IdentityService(IClient client)
        {
            Client = client;
        }

        public WhoamiResponse Whoami()
        {
            return new WhoamiResponse(Client.Get("/v2/whoami"));
        }
    }

    public class WhoamiResponse
    {
        public WhoamiData Data { get; }

        public WhoamiResponse(JToken response)
        {
            Data = new WhoamiData(response, InitializeSerializer());
        }

        // TODO: This wants to get out of there; soon...
        private static JsonSerializer InitializeSerializer()
        {
            JsonSerializer serializer = new JsonSerializer
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                DateParseHandling = DateParseHandling.DateTimeOffset
            };
            return serializer;
        }
    }

    public struct WhoamiData
    {
        private JsonSerializer Serializer { get; }
        public Account Account { get; }
        public User User { get; }

        public WhoamiData(JToken json, JsonSerializer serializer) : this()
        {
            Serializer = serializer;
            Account = AccountPart(json);
            User = UserPart(json);
        }

        private Account AccountPart(JToken json)
        {
            try
            {
                return json.SelectToken("data.account").ToObject<Account>(Serializer);
            }
            catch (JsonSerializationException) { return new Account(); }
        }

        private User UserPart(JToken json)
        {
            try
            {
                return json.SelectToken("data.user").ToObject<User>(Serializer);
            }
            catch (JsonSerializationException) { return new User(); }
        }
    }

    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct User
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct Account
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string PlanIdentifier { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}