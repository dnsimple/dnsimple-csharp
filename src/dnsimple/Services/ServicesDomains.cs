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
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <returns>The list of services applied to the domain.</returns>
        /// <see>https://developer.dnsimple.com/v2/services/domains/#listDomainAppliedServices</see>
        public PaginatedResponse<Service> AppliedServices(long accountId, string domainIdentifier)
        {
            var builder = BuildRequestForPath(AppliedServicesPath(accountId, domainIdentifier));

            return new PaginatedResponse<Service>(Execute(builder.Request));
        }

        /// <summary>
        /// Applies a service to a domain.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <param name="service">The service name or id</param>
        /// <returns><c>EmptyDnsimpleResponse</c></returns>
        /// <see>https://developer.dnsimple.com/v2/services/domains/#applyServiceToDomain</see>
        public EmptyResponse ApplyService(long accountId, string domainIdentifier, string service)
        {
            var builder = BuildRequestForPath(ApplyServicePath(accountId, domainIdentifier, service));
            builder.Method(Method.POST);

            return new EmptyResponse(Execute(builder.Request));
        }

        /// <summary>
        /// Un-applies a service from a domain.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <param name="service">The service name or id</param>
        /// <returns><c>EmptyDnsimpleResponse</c></returns>
        /// <see>https://developer.dnsimple.com/v2/services/domains/#unapplyServiceFromDomain</see>
        public EmptyResponse UnapplyService(long accountId, string domainIdentifier, string service)
        {
            var builder = BuildRequestForPath(ApplyServicePath(accountId, domainIdentifier, service));
            builder.Method(Method.DELETE);
            
            return new EmptyResponse(Execute(builder.Request));
        }
    }
}
