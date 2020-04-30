using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <inheritdoc cref="RegistrarService"/>
    /// <see>https://developer.dnsimple.com/v2/registrar/auto-renewal/</see>
    public partial class RegistrarService
    {
        /// <summary>
        /// Enables auto renewal for the domain.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="domain">The domain name or id</param>
        /// <see>https://developer.dnsimple.com/v2/registrar/auto-renewal/#enableDomainAutoRenewal</see>
        public EmptyDnsimpleResponse EnableDomainAutoRenewal(long accountId, string domain)
        {
            return DomainAutoRenewal(accountId, domain, Method.PUT);
        }
        
        /// <summary>
        /// Disables auto renewal for the domain.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="domain">The domain name or id</param>
        /// <see>https://developer.dnsimple.com/v2/registrar/auto-renewal/#disableDomainAutoRenewal</see>
        public EmptyDnsimpleResponse DisableDomainAutoRenewal(long accountId, string domain)
        {
            return DomainAutoRenewal(accountId, domain, Method.DELETE);
        }

        private EmptyDnsimpleResponse DomainAutoRenewal(long accountId, string domain,
            Method method)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(AutoRenewalPath(accountId, domain));
            requestBuilder.Method(method);
            return new EmptyDnsimpleResponse(Client.Http.Execute(requestBuilder.Request));
        }
    }
}