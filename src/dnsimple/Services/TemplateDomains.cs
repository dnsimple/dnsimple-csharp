using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    public partial class TemplatesService
    {
        /// <summary>
        /// Applies a template to a domain.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="domain">The domain name or id</param>
        /// <param name="template">The template id or short name (sid)</param>
        /// <returns><c>EmptyDnsimpleResponse</c></returns>
        /// <see>https://developer.dnsimple.com/v2/templates/domains/#applyTemplateToDomain</see>
        public EmptyResponse ApplyTemplate(long accountId,
            string domain, string template)
        {
            var builder = BuildRequestForPath(
                    TemplateDomainPath(accountId, domain, template));
            builder.Method(Method.POST);

            return new EmptyResponse(Execute(builder.Request));
        }
    }
}