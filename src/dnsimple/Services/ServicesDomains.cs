using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <inheritdoc cref="ServicesService"/>
    public partial class ServicesService
    {
        /// <summary>
        /// List services applied to a domain.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="domain">The domain name or id</param>
        /// <returns>The list of services applied to the domain.</returns>
        /// <see>https://developer.dnsimple.com/v2/services/domains/#listDomainAppliedServices</see>
        public PaginatedDnsimpleResponse<ServiceData> AppliedServices(
            long accountId, string domain)
        {
            return new PaginatedDnsimpleResponse<ServiceData>(
                Client.Http.Execute(Client.Http
                    .RequestBuilder(AppliedServicesPath(accountId, domain))
                    .Request));
        }

        /// <summary>
        /// Applies a service to a domain.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="domain">The domain name or id</param>
        /// <param name="service">The service name or id</param>
        /// <returns><c>EmptyDnsimpleResponse</c></returns>
        /// <see>https://developer.dnsimple.com/v2/services/domains/#applyServiceToDomain</see>
        public EmptyDnsimpleResponse ApplyService(long accountId, string domain,
            string service)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(ApplyServicePath(accountId, domain,
                    service));
            requestBuilder.Method(Method.POST);

            return new EmptyDnsimpleResponse(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Un-applies a service from a domain.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="domain">The domain name or id</param>
        /// <param name="service">The service name or id</param>
        /// <returns><c>EmptyDnsimpleResponse</c></returns>
        /// <see>https://developer.dnsimple.com/v2/services/domains/#unapplyServiceFromDomain</see>
        public EmptyDnsimpleResponse UnapplyService(long accountId, string domain, string service)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(ApplyServicePath(accountId, domain,
                    service));
            requestBuilder.Method(Method.DELETE);
            
            return new EmptyDnsimpleResponse(
                Client.Http.Execute(requestBuilder.Request));
        }
    }
}