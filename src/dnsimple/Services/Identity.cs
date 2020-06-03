using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace dnsimple.Services
{
    /// <summary>
    /// The <c>IdentityService</c> handles the identity (whoami) endpoint
    /// of the DNSimple API.
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/identity/</see>
    public class IdentityService : ServiceBase
    {
        /// <inheritdoc cref="ServiceBase"/>
        public IdentityService(IClient client) : base(client)
        {
        }

        /// <summary>
        /// Retrieves the details about the current authenticated entity used to access the API.
        /// </summary>
        /// <returns>
        /// A <c>WhoamiResponse</c> containing the User and/or Account.
        /// </returns>
        /// <see cref="Services.Whoami"/>
        /// <see cref="SimpleResponse{T}"/>
        /// <see>https://developer.dnsimple.com/v2/identity/#whoami</see>
        public SimpleResponse<Whoami> Whoami()
        {
            var builder = BuildRequestForPath("/whoami"); 

            return new SimpleResponse<Whoami>(Execute(builder.Request));
        }
    }

    /// <summary>
    /// Represents the <c>struct</c> containing a <c>User</c> and/or
    /// <c>Account</c> structure.
    /// </summary>
    /// <see cref="Account"/>
    /// <see cref="User"/>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)] 
    public struct Whoami
    {
        /// <summary>
        /// The instance of the <c>Account</c> <c>struct</c>.
        /// </summary>
        public Account Account { get; set; }
        
        /// <summary>
        /// The instance of the <c>User</c> <c>struct</c>.
        /// </summary>
        public User User { get; set; }
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
