using System;
using dnsimple.Services.ListOptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <summary>
    /// Handles communication with the template related methods of the
    /// DNSimple API.
    /// </summary>
    /// <see cref="Service"/>
    public partial class TemplatesService : Service
    {
        /// <inheritdoc cref="Service"/>
        public TemplatesService(IClient client) : base(client)
        {
        }

        /// <summary>
        /// List templates in the account.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <returns>A list of templates for the account</returns>
        /// <see>https://developer.dnsimple.com/v2/templates/#listTemplates</see>
        public PaginatedDnsimpleResponse<Template> ListTemplates(
            long accountId)
        {
            return new PaginatedDnsimpleResponse<Template>(
                Client.Http.Execute(Client.Http
                    .RequestBuilder(TemplatesPath(accountId)).Request));
        }

        /// <summary>
        /// List templates in the account.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="options">Options passed to the list (sorting and
        /// pagination).</param>
        /// <returns>A list of templates for the account</returns>
        /// <see>https://developer.dnsimple.com/v2/templates/#listTemplates</see>
        public PaginatedDnsimpleResponse<Template> ListTemplates(
            long accountId, ListTemplatesOptions options)
        {
            var requestBuilder = Client.Http
                .RequestBuilder(TemplatesPath(accountId));
            requestBuilder.AddParameter(options.UnpackSorting());

            if (!options.Pagination.IsDefault())
            {
                requestBuilder.AddParameters(options.UnpackPagination());
            }

            return new PaginatedDnsimpleResponse<Template>(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Create a template in the account
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="template">The template to be created</param>
        /// <returns>The newly created template</returns>
        /// <see>https://developer.dnsimple.com/v2/templates/#createTemplate</see>
        public SimpleDnsimpleResponse<Template> CreateTemplate(
            long accountId, Template template)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(TemplatesPath(accountId));
            requestBuilder.Method(Method.POST);
            requestBuilder.AddJsonPayload(template);

            return new SimpleDnsimpleResponse<Template>(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Retrieve a template
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="template">The template id or short name (sid)</param>
        /// <returns>The template requested</returns>
        /// <see>https://developer.dnsimple.com/v2/templates/#getTemplate</see>
        public SimpleDnsimpleResponse<Template> GetTemplate(long accountId,
            string template)
        {
            return new SimpleDnsimpleResponse<Template>(
                Client.Http.Execute(Client.Http
                    .RequestBuilder(TemplatePath(accountId, template))
                    .Request));
        }

        /// <summary>
        /// Update a template
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="template">The template id or shot name (sid)</param>
        /// <param name="payload">The <c>TemplateData</c> struct with the fields
        /// we want to update</param>
        /// <returns>The newly updated template</returns>
        /// <see>https://developer.dnsimple.com/v2/templates/#updateTemplate</see>
        public SimpleDnsimpleResponse<Template> UpdateTemplate(
            long accountId, string template, Template payload)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(TemplatePath(accountId, template));
            requestBuilder.Method(Method.PATCH);
            requestBuilder.AddJsonPayload(payload);

            return new SimpleDnsimpleResponse<Template>(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Deletes a template
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="template">The template name or id</param>
        /// <returns>An <c>EmptyDnsimpleResponse</c></returns>
        /// <see>https://developer.dnsimple.com/v2/templates/#deleteTemplate</see>
        public EmptyDnsimpleResponse DeleteTemplate(long accountId,
            string template)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(TemplatePath(accountId, template));
            requestBuilder.Method(Method.DELETE);

            return new EmptyDnsimpleResponse(
                Client.Http.Execute(requestBuilder.Request));
        }
    }

    /// <summary>
    /// Represents a Template in DNSimple.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy),
        ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct Template
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string Name { get; set; }
        public string Sid { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}