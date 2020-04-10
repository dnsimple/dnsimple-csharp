using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace dnsimple.Services
{
    /// <summary>
    /// The <c>IdentityService</c> handles the identity (whoami) endpoint
    /// of the DNSimple API.
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/identity/</see>
    public class IdentityService
    {
        private IClient Client { get; }

        /// <summary>
        /// Creates a new instance of a <c>IdentityService</c> by passing an
        /// instance of the DNSimple <c>IClient</c>.
        /// </summary>
        /// <param name="client"></param>
        public IdentityService(IClient client)
        {
            Client = client;
        }

        /// <summary>
        /// Sends a request to the DNSimple API <c>/whoami</c> endpoint.
        /// </summary>
        /// <returns>
        /// A <c>WhoamiResponse</c> containing the User and/or Account.
        /// </returns>
        /// <see cref="WhoamiResponse"/>
        /// <see cref="WhoamiData"/>
        public WhoamiResponse Whoami()
        {
            return new WhoamiResponse(
                Client.Http.Execute(Client.Http.RequestBuilder("/whoami")
                    .Request));
        }
    }

    /// <summary>
    /// Represents the <c>WhoamiResponse</c> containing the <c>WhoamiData</c>
    /// <c>struct</c>.
    /// </summary>
    public class WhoamiResponse
    {
        /// <summary>
        /// Represents the <c>struct</c> containing the data.
        /// </summary>
        public WhoamiData Data { get; }

        /// <summary>
        /// Constructs a new <c>WhoamiResponse</c> object with the
        /// <c>JToken</c> object returned from the API call.
        /// </summary>
        /// <param name="response"></param>
        public WhoamiResponse(JToken response)
            => Data = new WhoamiData(response, InitializeSerializer());

        // TODO: This wants to get out of there; soon...
        private static JsonSerializer InitializeSerializer()
        {
            var serializer = new JsonSerializer
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                DateParseHandling = DateParseHandling.DateTimeOffset
            };
            return serializer;
        }
    }

    /// <summary>
    /// Represents the <c>struct</c> containing a <c>User</c> and/or
    /// <c>Account</c> structure.
    /// </summary>
    /// <see cref="Account"/>
    /// <see cref="User"/>
    public readonly struct WhoamiData
    {
        /// <summary>
        /// The instance of the <c>Account</c> <c>struct</c>.
        /// </summary>
        public Account Account { get; }
        
        /// <summary>
        /// The instance of the <c>User</c> <c>struct</c>.
        /// </summary>
        public User User { get; }
        
        private JsonSerializer Serializer { get; }

        /// <summary>
        /// Constructs a new <c>WhoamiData</c> object by passing the
        /// <c>JToken</c> and <c>JsonSerializer</c>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The <c>JToken</c> object is a <c>JSON</c> object with the data
        /// from the call to the API.
        /// </para>
        /// <para>
        /// The <c>JsonSerializer</c> contains configurations to be used when
        /// deserializing the raw data into a <c>Account</c> and/or <c>User</c>
        /// objects.
        /// </para>
        /// </remarks>
        /// <param name="json"></param>
        /// <param name="serializer"></param>
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
                return json.SelectToken("data.account")
                    .ToObject<Account>(Serializer);
            }
            catch (Exception)
            {
                //TODO: Handle these Exceptions properly
                return new Account();
            }
        }

        private User UserPart(JToken json)
        {
            try
            {
                return json.SelectToken("data.user").ToObject<User>(Serializer);
            }
            catch (Exception)
            {
                //TODO: Handle these Exceptions properly
                return new User();
            }
        }
    }

    /// <summary>
    /// Represents a <c>User</c>
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/identity/</see>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct User
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Represents an <c>Account</c>
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/identity/</see>
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