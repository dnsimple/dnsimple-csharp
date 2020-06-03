using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    public partial class TemplatesService
    {
        /// <summary>
        /// Applies a template to a domain.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <param name="templateIdentifier">The template ID or short name (SID)</param>
        /// <see>https://developer.dnsimple.com/v2/templates/domains/#applyTemplateToDomain</see>
        public EmptyResponse ApplyTemplate(long accountId, string domainIdentifier, string templateIdentifier)
        {
            var builder = BuildRequestForPath(TemplateDomainPath(accountId, domainIdentifier, templateIdentifier));
            builder.Method(Method.POST);

            return new EmptyResponse(Execute(builder.Request));
        }
    }
}
