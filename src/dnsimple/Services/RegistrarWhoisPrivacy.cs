using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <summary>
    /// Enable and disable WHOIS privacy on registered domains.
    /// </summary>
    /// <inheritdoc cref="RegistrarService"/>
    /// <see>https://developer.dnsimple.com/v2/registrar/whois-privacy/</see>
    public partial class RegistrarService
    {
        /// <summary>
        /// Gets the whois privacy status for an existing domain.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domain">The domain name</param>
        /// <returns>The WHOIS privacy for the domain</returns>
        /// <see>https://developer.dnsimple.com/v2/registrar/whois-privacy/#getWhoisPrivacy</see>
        public SimpleResponse<WhoisPrivacy> GetWhoisPrivacy(long accountId, string domain)
        {
            var builder = BuildRequestForPath(WhoisPrivacyPath(accountId, domain)); 

            return new SimpleResponse<WhoisPrivacy>(Execute(builder.Request));
        }

        /// <summary>
        /// Enables the WHOIS privacy for the domain.
        /// </summary>
        /// <remarks>
        /// Note that if the WHOIS privacy is not purchased for the domain,
        /// enabling WHOIS privacy will cause the service to be purchased for
        /// a period of 1 year.
        ///
        /// If WHOIS privacy was previously purchased and disabled, then
        /// calling this will enable the WHOIS privacy.
        /// </remarks>
        /// <param name="accountId">The account ID</param>
        /// <param name="domain">The domain name</param>
        /// <returns>The WHOIS privacy for the domain</returns>
        /// <see>https://developer.dnsimple.com/v2/registrar/whois-privacy/#enableWhoisPrivacy</see>
        public SimpleResponse<WhoisPrivacy> EnableWhoisPrivacy(long accountId, string domain)
        {
            var builder = BuildRequestForPath(WhoisPrivacyPath(accountId, domain));
            builder.Method(Method.PUT);

            return new SimpleResponse<WhoisPrivacy>(Execute(builder.Request));
        }

        /// <summary>
        /// Disables the WHOIS privacy for the domain.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domain">The domain name</param>
        /// <returns>The WHOIS privacy response for the domain</returns>
        /// <see>https://developer.dnsimple.com/v2/registrar/whois-privacy/#disableWhoisPrivacy</see>
        public SimpleResponse<WhoisPrivacy> DisableWhoisPrivacy(long accountId, string domain)
        {
            var builder = BuildRequestForPath(WhoisPrivacyPath(accountId, domain));
            builder.Method(Method.DELETE);

            return new SimpleResponse<WhoisPrivacy>(Execute(builder.Request));
        }

        /// <summary>
        /// Renews the WHOIS privacy for the domain.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domain">The domain name</param>
        /// <returns>A whois renewal response with the renewal information</returns>
        /// <see>https://developer.dnsimple.com/v2/registrar/whois-privacy/#renewWhoisPrivacy</see>
        public SimpleResponse<WhoisPrivacyRenewal> RenewWhoisPrivacy(long accountId, string domain)
        {
            var builder = BuildRequestForPath(WhoisRenewalPath(accountId, domain));
            builder.Method(Method.POST);

            return new SimpleResponse<WhoisPrivacyRenewal>(Execute(builder.Request));
        }
    }

    /// <summary>
    /// Represents the WHOIS privacy data for a domain.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct WhoisPrivacy
    {
        public long Id { get; set; }
        public long DomainId { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public bool? Enabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Represents the WHOIS privacy renewal data for a domain.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct WhoisPrivacyRenewal
    {
        public long Id { get; set; }
        public long DomainId { get; set; }
        public long WhoisPrivacyId { get; set; }
        public string State { get; set; }
        public DateTime? ExpiresOn { get; set; }
        public bool? Enabled { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
