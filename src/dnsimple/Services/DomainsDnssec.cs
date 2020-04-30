using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <inheritdoc cref="DomainsService"/>
    /// <see>https://developer.dnsimple.com/v2/domains/dnssec/</see>
    public partial class DomainsService
    {
        /// <summary>
        /// Get the status of DNSSEC, indicating whether it is currently enabled or disabled.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <returns>The status of the DNSSEC wrapped in a response</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/dnssec/#getDomainDnssec</see>
        public SimpleDnsimpleResponse<DnssecStatus> GetDnssec(long accountId,
            string domainIdentifier)
        {
            return new SimpleDnsimpleResponse<DnssecStatus>(Client.Http.Execute(Client.Http
                .RequestBuilder(DnssecPath(accountId, domainIdentifier))
                .Request));
        }

        /// <summary>
        /// Enable DNSSEC for the domain in the account. This will sign the
        /// zone. If the domain is registered it will also add the DS record
        /// to the corresponding registry.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <returns>The confirmation of the operation withe the status of the
        /// DNSSEC wrapped in a response</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/dnssec/#enableDomainDnssec</see>
        public SimpleDnsimpleResponse<DnssecStatus> EnableDnssec(long accountId,
            string domainIdentifier)
        {
            var request =
                Client.Http.RequestBuilder(
                    DnssecPath(accountId, domainIdentifier));
            request.Method(Method.POST);

            return new SimpleDnsimpleResponse<DnssecStatus>(Client.Http.Execute(request.Request));
        }

        /// <summary>
        /// Disable DNSSEC for the domain in the account.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <remarks>Will throw a <c>DnSimpleException</c> if trying to
        /// disable DNSSEC for a domain that hasn't DNSSEC enabled.</remarks>
        /// <see cref="DnSimpleException"/>
        /// <see>https://developer.dnsimple.com/v2/domains/dnssec/#disableDomainDnssec</see>
        public EmptyDnsimpleResponse DisableDnssec(long accountId, string domainIdentifier)
        {
            var request =
                Client.Http.RequestBuilder(DnssecPath(accountId,
                    domainIdentifier));
            request.Method(Method.DELETE);
            
            return new EmptyDnsimpleResponse(Client.Http.Execute(request.Request));
        }
    }

    /// <summary>
    /// Represents the DNSSEC status
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/domains/dnssec/</see>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct DnssecStatus
    {
        public bool Enabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}