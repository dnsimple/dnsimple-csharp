using System;
using dnsimple.Services.ListOptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <inheritdoc cref="TemplatesService"/>
    public partial class TemplatesService
    {
        /// <summary>
        /// List records for the template.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="template">The template id or short name (sid)</param>
        /// <returns>The list of template records</returns>
        /// <see>https://developer.dnsimple.com/v2/templates/records/#listTemplateRecords</see>
        public PaginatedDnsimpleResponse<TemplateRecord> ListTemplateRecords(
            long accountId, string template)
        {
            return new PaginatedDnsimpleResponse<TemplateRecord>(
                Client.Http.Execute(Client.Http
                    .RequestBuilder(TemplateRecordsPath(accountId, template))
                    .Request));
        }

        /// <summary>
        /// List records for the template.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="template">The template id or short name (sid)</param>
        /// <param name="options">Options passed to the list (sorting and
        /// pagination).</param>
        /// <returns>The list of template records</returns>
        /// <see>https://developer.dnsimple.com/v2/templates/records/#listTemplateRecords</see>
        public PaginatedDnsimpleResponse<TemplateRecord> ListTemplateRecords(
            long accountId, string template, ListTemplateRecordsOptions options)
        {
            var requestBuilder = Client.Http
                .RequestBuilder(TemplateRecordsPath(accountId, template));
            requestBuilder.AddParameter(options.UnpackSorting());

            if (!options.Pagination.IsDefault())
            {
                requestBuilder.AddParameters(options.UnpackPagination());
            }

            return new PaginatedDnsimpleResponse<TemplateRecord>(
                Client.Http.Execute(requestBuilder
                    .Request));
        }

        /// <summary>
        /// Create a record for the template in the account
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="template">The template id or short name (sid)</param>
        /// <param name="payload">The <c>TemplateRecord</c> to create</param>
        /// <returns>The newly created <c>TemplateRecord</c></returns>
        /// <see cref="TemplateRecord"/>
        /// <see>https://developer.dnsimple.com/v2/templates/records/#createTemplateRecord</see>
        public SimpleDnsimpleResponse<TemplateRecord> CreateTemplateRecord(long
            accountId, string template, TemplateRecord payload)
        {
            var requestBuilder = Client.Http
                .RequestBuilder(TemplateRecordsPath(accountId, template));
            requestBuilder.Method(Method.POST);
            requestBuilder.AddJsonPayload(payload);

            return new SimpleDnsimpleResponse<TemplateRecord>(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Gets the record for the template in the account.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="template">The template id or short name (sid)</param>
        /// <param name="recordId">The record Id</param>
        /// <returns>The <c>TemplateRecord</c> requested</returns>
        /// <see>https://developer.dnsimple.com/v2/templates/records/#getTemplateRecord</see>
        public SimpleDnsimpleResponse<TemplateRecord> GetTemplateRecord(
            long accountId, string template, long recordId)
        {
            return new SimpleDnsimpleResponse<TemplateRecord>(
                Client.Http.Execute(Client.Http
                    .RequestBuilder(TemplateRecordPath(accountId, template,
                        recordId)).Request));
        }

        /// <summary>
        /// Delete the record for template in the account.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="template">The template id or short name (sid)</param>
        /// <param name="recordId">The record Id</param>
        /// <returns><c>EmptyDnsimpleResponse</c></returns>
        /// <see>https://developer.dnsimple.com/v2/templates/records/#deleteTemplateRecord</see>
        public EmptyDnsimpleResponse DeleteTemplateRecord(long accountId, string template, long recordId)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(
                    TemplateRecordPath(accountId, template, recordId));
            requestBuilder.Method(Method.DELETE);
            
            return new EmptyDnsimpleResponse(Client.Http.Execute(requestBuilder.Request));
        }
    }

    /// <summary>
    /// Represents a DNS record for a template in DNSimple.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy),
        ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct TemplateRecord
    {
        public long Id { get; set; }
        public long TemplateId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public long Ttl { get; set; }
        public long? Priority { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}