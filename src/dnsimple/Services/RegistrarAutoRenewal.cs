using RestSharp;

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
        public void EnableDomainAutoRenewal(long accountId, string domain)
        {
            DomainAutoRenewal(accountId, domain, Method.PUT);
        }
        
        /// <summary>
        /// Disables auto renewal for the domain.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="domain">The domain name or id</param>
        /// <see>https://developer.dnsimple.com/v2/registrar/auto-renewal/#disableDomainAutoRenewal</see>
        public void DisableDomainAutoRenewal(long accountId, string domain)
        {
            DomainAutoRenewal(accountId, domain, Method.DELETE);
        }

        private void DomainAutoRenewal(long accountId, string domain,
            Method method)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(AutoRenewalPath(accountId, domain));
            requestBuilder.Method(method);
            Client.Http.Execute(requestBuilder.Request);
        }

        private string AutoRenewalPath(long accountId, string domain)
        {
            return $"{RegistrarPath(accountId, domain)}/auto_renewal";
        }
    }
}