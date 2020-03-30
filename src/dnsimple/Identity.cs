using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

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
            Data = new WhoamiData(response);
        }
    }

    public struct WhoamiData
    {
        public Account Account { get; }

        public WhoamiData(JToken json)
        {
            Account = JsonConvert.DeserializeObject<Account>(AccountPart(json));
        }

        private static string AccountPart(JToken json)
        {
            return json.SelectToken("data.account").ToString();
        }
    }

    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct Account
    {
        public long Id { get; set; }

        public string Email { get; set; }

        public string PlanIdentifier { get; set; }

        public string CreatedAt { get; set; }

        public string UpdatedAt { get; set; }

        public Account(long id, string email, string planIdentifier,
            string createdAt, string updatedAt)
        {
            Id = id;
            Email = email;
            PlanIdentifier = planIdentifier;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
        }
    }
}