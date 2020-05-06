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
        /// <param name="options">Options passed to the list (sorting and
        /// pagination).</param>
        /// <returns>The list of template records</returns>
        /// <see>https://developer.dnsimple.com/v2/templates/records/#listTemplateRecords</see>
        public PaginatedDnsimpleResponse<TemplateRecord> ListTemplateRecords(
            long accountId, string template,
            ListTemplateRecordsOptions options = null)
        {
            var builder =
                BuildRequestForPath(TemplateRecordsPath(accountId, template));

            AddListOptionsToRequest(options, ref builder);

            return new PaginatedDnsimpleResponse<TemplateRecord>(Execute(builder
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
            var builder =
                BuildRequestForPath(TemplateRecordsPath(accountId, template));
            builder.Method(Method.POST);
            builder.AddJsonPayload(payload);

            return new SimpleDnsimpleResponse<TemplateRecord>(
                Execute(builder.Request));
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
            return new SimpleDnsimpleResponse<TemplateRecord>(Execute(
                BuildRequestForPath(TemplateRecordPath(accountId, template,
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
        public EmptyDnsimpleResponse DeleteTemplateRecord(long accountId,
            string template, long recordId)
        {
            var builder = BuildRequestForPath(
                    TemplateRecordPath(accountId, template, recordId));
            builder.Method(Method.DELETE);

            return new EmptyDnsimpleResponse(Execute(builder.Request));
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
        
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }
        
        [JsonProperty(Required = Required.Always)]
        public string Content { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Type { get; set; }

        public long Ttl { get; set; }
        public long? Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}