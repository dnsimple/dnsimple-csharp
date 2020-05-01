using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <inheritdoc cref="Service"/>
    /// <see>https://developer.dnsimple.com/v2/vanity/</see>
    public class VanityNameServersService : Service
    {
        /// <inheritdoc cref="Service"/>
        public VanityNameServersService(IClient client) : base(client)
        {
        }

        /// <summary>
        /// Enable vanity name servers.
        /// <para>
        /// This method sets up the appropriate A and AAAA records for the
        /// domain to provide vanity name servers, but it does not change the
        /// delegation for the domain.
        /// </para>
        /// <para>
        /// To change the delegation for domains to vanity name servers use
        /// <c>RegistrarService</c> (<c>ChangeDomainDelegationToVanity()</c>,
        /// <c>ChangeDomainDelegationFromVanity()</c>) to Delegate to Vanity Name
        /// Servers or Delegate from Vanity Name Servers.
        /// </para>
        /// </summary>
        /// <remarks>
        /// The vanity name servers is a feature that is only available to the
        /// following new plans: Business. If the feature is not enabled, a
        /// <c>DnsimpleException</c> will be thrown.
        /// </remarks>
        /// <param name="accountId">The account id</param>
        /// <param name="domain">Tne domain name or id</param>
        /// <returns>The vanity name server list</returns>
        /// <see>https://developer.dnsimple.com/v2/vanity/#enableVanityNameServers</see>
        public ListDnsimpleResponse<VanityNameServer> EnableVanityNameServers(
            long accountId, string domain)
        {
            var builder = BuildRequestForPath(
                VanityNameServersPath(accountId, domain));
            builder.Method(Method.PUT);

            return new ListDnsimpleResponse<VanityNameServer>(
                Execute(builder.Request));
        }

        /// <summary>
        /// Disable Vanity Name Servers for the domain.
        /// <para>This method removes the A and AAAA records required for the
        /// domain to provide vanity name servers, but it does not change
        /// the delegation for the domain.
        /// </para>
        /// <para>
        /// To change the delegation for domains to vanity name servers use
        /// <c>RegistrarService</c> (<c>ChangeDomainDelegationToVanity()</c>,
        /// <c>ChangeDomainDelegationFromVanity()</c>) to Delegate to Vanity Name
        /// Servers or Delegate from Vanity Name Servers.
        /// </para>
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="domain">The domain name or id</param>
        /// <returns><c>EmptyDnsimpleResponse</c></returns>
        public EmptyDnsimpleResponse DisableVanityNameServers(long accountId,
            string domain)
        {
            var builder = BuildRequestForPath(
                    VanityNameServersPath(accountId, domain));
            builder.Method(Method.DELETE);

            return new EmptyDnsimpleResponse(Execute(builder.Request));
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