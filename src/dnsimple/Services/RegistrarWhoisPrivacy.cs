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
        /// Get the WHOIS privacy details for a domain.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="domain">The domain name or id</param>
        /// <returns>The WHOIS privacy for the domain</returns>
        /// <see>https://developer.dnsimple.com/v2/registrar/whois-privacy/#getWhoisPrivacy</see>
        public SimpleDnsimpleResponse<WhoisPrivacyData> GetWhoisPrivacy(long accountId,
            string domain)
        {
            return new SimpleDnsimpleResponse<WhoisPrivacyData>(Client.Http.Execute(Client.Http
                .RequestBuilder(WhoisPrivacyPath(accountId, domain)).Request));
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
        /// <param name="accountId">The account Id</param>
        /// <param name="domain">The domain name or id</param>
        /// <returns>The WHOIS privacy for the domain</returns>
        /// <see>https://developer.dnsimple.com/v2/registrar/whois-privacy/#enableWhoisPrivacy</see>
        public SimpleDnsimpleResponse<WhoisPrivacyData> EnableWhoisPrivacy(long accountId,
            string domain)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(WhoisPrivacyPath(accountId, domain));
            requestBuilder.Method(Method.PUT);
            
            return new SimpleDnsimpleResponse<WhoisPrivacyData>(Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Disable WHOIS privacy for the domain.
        /// </summary>
        /// <remarks>
        /// Note that if the WHOIS privacy is not purchased for the domain,
        /// this method will do nothing.
        /// If WHOIS privacy was previously purchased and enabled, then calling
        /// this will disable the WHOIS privacy.
        /// </remarks>
        /// <param name="accountId">The account Id</param>
        /// <param name="domain">The domain name or id</param>
        /// <returns>The WHOIS privacy response for the domain</returns>
        /// <see>https://developer.dnsimple.com/v2/registrar/whois-privacy/#disableWhoisPrivacy</see>
        public SimpleDnsimpleResponse<WhoisPrivacyData> DisableWhoisPrivacy(long accountId,
            string domain)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(WhoisPrivacyPath(accountId, domain));
            requestBuilder.Method(Method.DELETE);
            
            return new SimpleDnsimpleResponse<WhoisPrivacyData>(Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Renew whois privacy for the domain.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="domain">The domain name or id</param>
        /// <returns>A whois renewal response with the renewal information</returns>
        /// <see>https://developer.dnsimple.com/v2/registrar/whois-privacy/#renewWhoisPrivacy</see>
        public SimpleDnsimpleResponse<WhoisPrivacyRenewalData> RenewWhoisPrivacy(long accountId,
            string domain)
        {
            var requestBuilder = Client.Http.RequestBuilder(WhoisRenewalPath(accountId, domain));
            requestBuilder.Method(Method.POST);
            
            return new SimpleDnsimpleResponse<WhoisPrivacyRenewalData>(Client.Http.Execute(requestBuilder.Request));
        }
    }

    /// <summary>
    /// Represents the WHOIS privacy data for a domain.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct WhoisPrivacyData
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
    public struct WhoisPrivacyRenewalData
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