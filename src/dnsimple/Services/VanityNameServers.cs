using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <inheritdoc cref="ServiceBase"/>
    /// <see>https://developer.dnsimple.com/v2/vanity/</see>
    public class VanityNameServersService : ServiceBase
    {
        /// <inheritdoc cref="ServiceBase"/>
        public VanityNameServersService(IClient client) : base(client)
        {
        }

        /// <summary>
        /// Enables Vanity Name Servers for the domain.
        /// </summary>
        /// <remarks>
        /// The vanity name servers feature is only available on certain plans.
        /// If the feature is not enabled for your plan, a <c>DnsimpleException</c> will be thrown.
        /// </remarks>
        /// <param name="accountId">The account ID</param>
        /// <param name="domain">Tne domain name or id</param>
        /// <returns>The vanity name server list</returns>
        /// <see>https://developer.dnsimple.com/v2/vanity/#enableVanityNameServers</see>
        public ListResponse<VanityNameServer> EnableVanityNameServers(long accountId, string domain)
        {
            var builder = BuildRequestForPath(VanityNameServersPath(accountId, domain));
            builder.Method(Method.PUT);

            return new ListResponse<VanityNameServer>(Execute(builder.Request));
        }

        /// <summary>
        /// Disables Vanity Name Servers for the domain.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domain">The domain name or id</param>
        public EmptyResponse DisableVanityNameServers(long accountId, string domain)
        {
            var builder = BuildRequestForPath(VanityNameServersPath(accountId, domain));
            builder.Method(Method.DELETE);

            return new EmptyResponse(Execute(builder.Request));
        }
    }

    /// <summary>
    /// Represents data for a single vanity name server.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct VanityNameServer
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Ipv4 { get; set; }
        public string Ipv6 { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
