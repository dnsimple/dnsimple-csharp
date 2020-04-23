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
    public class IdentityService : Service
    {

        /// <inheritdoc cref="Service"/>
        public IdentityService(IClient client) : base(client) {}
        
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
    public class WhoamiResponse : SimpleDnsimpleResponse<WhoamiData>
    {
        
        /// <summary>
        /// Constructs a new <c>WhoamiResponse</c> object with the
        /// <c>JToken</c> object returned from the API call.
        /// </summary>
        /// <param name="json"></param>
        public WhoamiResponse(JToken json) : base(json) => 
            Data = new WhoamiData(json);
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
        public WhoamiData(JToken json) : this()
        {
            Account = AccountPart(json);
            User = UserPart(json);
        }

        private static Account AccountPart(JToken json)
        {
            try
            {
                return JsonTools<Account>.DeserializeObject("data.account",
                    json);
            }
            catch (Exception)
            {
                return new Account();
            }
        }

        private static User UserPart(JToken json)
        {
            try
            {
                return JsonTools<User>.DeserializeObject("data.user", json);
            }
            catch (Exception)
            {
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