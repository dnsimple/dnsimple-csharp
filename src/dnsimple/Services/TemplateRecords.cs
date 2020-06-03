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
        /// Lists the records for a template.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="template">The template id or short name (sid)</param>
        /// <param name="options">Options passed to the list (sorting and
        /// pagination).</param>
        /// <returns>The list of template records</returns>
        /// <see>https://developer.dnsimple.com/v2/templates/records/#listTemplateRecords</see>
        public PaginatedResponse<TemplateRecord> ListTemplateRecords(long accountId, string template, ListTemplateRecordsOptions options = null)
        {
            var builder = BuildRequestForPath(TemplateRecordsPath(accountId, template));
            AddListOptionsToRequest(options, ref builder);

            return new PaginatedResponse<TemplateRecord>(Execute(builder.Request));
        }

        /// <summary>
        /// Creates a new template record.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="template">The template id or short name (sid)</param>
        /// <param name="payload">The <c>TemplateRecord</c> to create</param>
        /// <returns>The newly created <c>TemplateRecord</c></returns>
        /// <see cref="TemplateRecord"/>
        /// <see>https://developer.dnsimple.com/v2/templates/records/#createTemplateRecord</see>
        public SimpleResponse<TemplateRecord> CreateTemplateRecord(long accountId, string template, TemplateRecord payload)
        {
            var builder = BuildRequestForPath(TemplateRecordsPath(accountId, template));
            builder.Method(Method.POST);
            builder.AddJsonPayload(payload);

            return new SimpleResponse<TemplateRecord>(Execute(builder.Request));
        }

        /// <summary>
        /// Retrieves the details of an existing template record.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="template">The template id or short name (sid)</param>
        /// <param name="recordId">The record Id</param>
        /// <returns>The <c>TemplateRecord</c> requested</returns>
        /// <see>https://developer.dnsimple.com/v2/templates/records/#getTemplateRecord</see>
        public SimpleResponse<TemplateRecord> GetTemplateRecord(long accountId, string template, long recordId)
        {
            var builder = BuildRequestForPath(TemplateRecordPath(accountId, template, recordId)); 
            
            return new SimpleResponse<TemplateRecord>(Execute(builder.Request));
        }

        /// <summary>
        /// Permanently deletes a template record.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="template">The template id or short name (sid)</param>
        /// <param name="recordId">The record Id</param>
        /// <see>https://developer.dnsimple.com/v2/templates/records/#deleteTemplateRecord</see>
        public EmptyResponse DeleteTemplateRecord(long accountId, string template, long recordId)
        {
            var builder = BuildRequestForPath(TemplateRecordPath(accountId, template, recordId));
            builder.Method(Method.DELETE);

            return new EmptyResponse(Execute(builder.Request));
        }
    }

    /// <summary>
    /// Represents a DNS record for a template in DNSimple.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
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
