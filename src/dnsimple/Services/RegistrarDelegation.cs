using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <inheritdoc cref="RegistrarService"/>
    /// <see>https://developer.dnsimple.com/v2/registrar/delegation/</see>
    public partial class RegistrarService
    {
        /// <summary>
        /// List name servers for the domain in the account.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="domain">The domain id or name</param>
        /// <returns>The list of name servers for the domain</returns>
        /// <see>https://developer.dnsimple.com/v2/registrar/delegation/#getDomainDelegation</see>
        public DelegationResponse GetDomainDelegation(long accountId,
            string domain)
        {
            return new DelegationResponse(
                Client.Http.Execute(Client.Http
                    .RequestBuilder(DelegationPath(accountId, domain))
                    .Request));
        }

        /// <summary>
        /// Update name servers for the domain.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="domain">The domain id or name</param>
        /// <param name="delegation">A list of name server names as strings</param>
        /// <returns>The list of updated name servers for the domain</returns>
        /// <see>https://developer.dnsimple.com/v2/registrar/delegation/#changeDomainDelegation</see>
        public DelegationResponse ChangeDomainDelegation(long accountId,
            string domain, IList<string> delegation)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(
                    DelegationPath(accountId, domain));
            requestBuilder.Method(Method.PUT);
            requestBuilder.AddJsonPayload(delegation);

            return new DelegationResponse(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Update name servers for the domain.
        /// </summary>
        /// <remarks>
        /// This method required the vanity name servers feature, that
        /// is only available for certain plans. If the feature is not enabled,
        /// you will receive an HTTP 412 response code.
        /// </remarks>
        /// <param name="accountId">The account Id.</param>
        /// <param name="domain">The domain id or name</param>
        /// <param name="delegation">A list of name servers as strings</param>
        /// <returns>The list of nameservers updated to vanity for the domain</returns>
        /// <see>https://developer.dnsimple.com/v2/registrar/delegation/#changeDomainDelegationToVanity</see>
        public ListDnsimpleResponse<VanityDelegation> ChangeDomainDelegationToVanity(
            long accountId, string domain, List<string> delegation)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(
                    VanityDelegationPath(accountId, domain));
            requestBuilder.Method(Method.PUT);
            requestBuilder.AddJsonPayload(delegation);
            
            return new ListDnsimpleResponse<VanityDelegation>(Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Update name servers for the domain from vanity.
        /// </summary>
        /// <remarks>
        /// This method required the vanity name servers feature, that is only
        /// available for certain plans. If the feature is not enabled,
        /// you will receive an HTTP 412 response code.
        /// </remarks>
        /// <param name="accountId">The account Id</param>
        /// <param name="domain">The domain id or name</param>
        /// <see>https://developer.dnsimple.com/v2/registrar/delegation/#changeDomainDelegationFromVanity</see>
        public void ChangeDomainDelegationFromVanity(long accountId, string domain)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(
                    VanityDelegationPath(accountId, domain));
            requestBuilder.Method(Method.DELETE);

            Client.Http.Execute(requestBuilder.Request);
        }
    }

    /// <summary>
    /// Represents the response from the API call to get the domain delegation.
    /// </summary>
    public class DelegationResponse
    {
        public List<string> Data { get; private set; }

        public DelegationResponse(JToken json)
        {
            Data = JsonTools<string>.DeserializeList(json);
        }
    }

    /// <summary>
    /// Represents a vanity delegation
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct VanityDelegation
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Ipv4 { get; set; }
        public string Ipv6 { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
}