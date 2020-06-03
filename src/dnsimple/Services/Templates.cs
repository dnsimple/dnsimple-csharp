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
    /// <see cref="ServiceBase"/>
    public partial class TemplatesService : ServiceBase
    {
        /// <inheritdoc cref="ServiceBase"/>
        public TemplatesService(IClient client) : base(client)
        {
        }

        /// <summary>
        /// Lists the templates in the account.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="options">Options passed to the list (sorting and
        /// pagination).</param>
        /// <returns>A list of templates for the account</returns>
        /// <see>https://developer.dnsimple.com/v2/templates/#listTemplates</see>
        public PaginatedResponse<Template> ListTemplates(long accountId, ListTemplatesOptions options = null)
        {
            var builder = BuildRequestForPath(TemplatesPath(accountId));

            AddListOptionsToRequest(options, ref builder);

            return new PaginatedResponse<Template>(Execute(builder.Request));
        }

        /// <summary>
        /// Creates a template.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="template">The template to be created</param>
        /// <returns>The newly created template</returns>
        /// <see>https://developer.dnsimple.com/v2/templates/#createTemplate</see>
        public SimpleResponse<Template> CreateTemplate(long accountId, Template template)
        {
            var builder = BuildRequestForPath(TemplatesPath(accountId));
            builder.Method(Method.POST);
            builder.AddJsonPayload(template);

            return new SimpleResponse<Template>(Execute(builder.Request));
        }

        /// <summary>
        /// Retrieves the details of an existing template.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="templateIdentifier">The template ID or short name (SID)</param>
        /// <returns>The template requested</returns>
        /// <see>https://developer.dnsimple.com/v2/templates/#getTemplate</see>
        public SimpleResponse<Template> GetTemplate(long accountId, string templateIdentifier)
        {
            var builder = BuildRequestForPath(TemplatePath(accountId, templateIdentifier));
            
            return new SimpleResponse<Template>(Execute(builder.Request));
        }

        /// <summary>
        /// Updates the template details.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="templateIdentifier">The template ID or short name (SID)</param>
        /// <param name="payload">The <c>Template</c> struct with the fields
        /// we want to update</param>
        /// <returns>The newly updated template</returns>
        /// <see>https://developer.dnsimple.com/v2/templates/#updateTemplate</see>
        public SimpleResponse<Template> UpdateTemplate(long accountId, string templateIdentifier, Template payload)
        {
            var builder = BuildRequestForPath(TemplatePath(accountId, templateIdentifier));
            builder.Method(Method.PATCH);
            builder.AddJsonPayload(payload);

            return new SimpleResponse<Template>(Execute(builder.Request));
        }

        /// <summary>
        /// Permanently deletes a template.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="templateIdentifier">The template ID or short name (SID)</param>
        /// <returns>An <c>EmptyDnsimpleResponse</c></returns>
        /// <see>https://developer.dnsimple.com/v2/templates/#deleteTemplate</see>
        public EmptyResponse DeleteTemplate(long accountId, string templateIdentifier)
        {
            var builder = BuildRequestForPath(TemplatePath(accountId, templateIdentifier));
            builder.Method(Method.DELETE);

            return new EmptyResponse(Execute(builder.Request));
        }
    }

    /// <summary>
    /// Represents a Template in DNSimple.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct Template
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Sid { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
