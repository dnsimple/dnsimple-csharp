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
        /// Checks the DNSSEC status for an existing domain.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <returns>The status of the DNSSEC wrapped in a response</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/dnssec/#getDomainDnssec</see>
        public SimpleResponse<DnssecStatus> GetDnssec(long accountId, string domainIdentifier)
        {
            var builder = BuildRequestForPath(DnssecPath(accountId, domainIdentifier)); 

            return new SimpleResponse<DnssecStatus>(Execute(builder.Request));
        }

        /// <summary>
        /// Enables DNSSEC for the domain.
        ///
        /// It will enable signing of the zone. If the domain is registered with DNSimple,
        /// it will also add the DS record to the corresponding registry.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <returns>The confirmation of the operation withe the status of the
        /// DNSSEC wrapped in a response</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/dnssec/#enableDomainDnssec</see>
        public SimpleResponse<DnssecStatus> EnableDnssec(long accountId, string domainIdentifier)
        {
            var builder = BuildRequestForPath(DnssecPath(accountId, domainIdentifier));
            builder.Method(Method.POST);

            return new SimpleResponse<DnssecStatus>(Execute(builder.Request));
        }

        /// <summary>
        /// Disables DNSSEC for the domain.
        ///
        /// It will disable signing of the zone. If the domain is registered with DNSimple,
        /// it will also remove the DS record at the registry corresponding to the disabled DNSSEC signing.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <remarks>Will throw a <c>DnSimpleException</c> if trying to
        /// disable DNSSEC for a domain that hasn't DNSSEC enabled.</remarks>
        /// <see cref="DnSimpleException"/>
        /// <see>https://developer.dnsimple.com/v2/domains/dnssec/#disableDomainDnssec</see>
        public EmptyResponse DisableDnssec(long accountId, string domainIdentifier)
        {
            var builder = BuildRequestForPath(DnssecPath(accountId,domainIdentifier));
            builder.Method(Method.DELETE);

            return new EmptyResponse(Execute(builder.Request));
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
