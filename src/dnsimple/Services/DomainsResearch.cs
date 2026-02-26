using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <summary>
    /// Domain research API: availability and registration status.
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/domains/research/</see>
    public partial class DomainsService
    {
        /// <summary>
        /// Research a domain name for availability and registration status information.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainName">The domain name to research</param>
        /// <returns>The domain research status response</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/research/#getDomainsResearchStatus</see>
        public SimpleResponse<DomainResearchStatus> GetDomainResearchStatus(long accountId, string domainName)
        {
            var builder = BuildRequestForPath(DomainsResearchStatusPath(accountId));
            builder.AddParameter(new KeyValuePair<string, string>("domain", domainName));
            return new SimpleResponse<DomainResearchStatus>(Execute(builder.Request));
        }
    }

    /// <summary>
    /// Represents the result of a domain research status request.
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/domains/research/#getDomainsResearchStatus</see>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy),
        ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct DomainResearchStatus
    {
        /// <summary>UUID identifier for this research request.</summary>
        public string RequestId { get; set; }

        /// <summary>The domain name that was researched.</summary>
        public string Domain { get; set; }

        /// <summary>The availability status (e.g. "available", "unavailable").</summary>
        public string Availability { get; set; }

        /// <summary>Array of error messages if the domain cannot be researched.</summary>
        public List<string> Errors { get; set; }
    }
}
