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
        public EmptyDnsimpleResponse ApplyTemplate(long accountId,
            string domain, string template)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(
                    TemplateDomainPath(accountId, domain, template));
            requestBuilder.Method(Method.POST);

            return new EmptyDnsimpleResponse(
                Client.Http.Execute(requestBuilder.Request));
        }
    }
}