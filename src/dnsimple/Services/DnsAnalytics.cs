using System.Collections.Generic;
using System.Linq;
using dnsimple.Services.ListOptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <summary>
    /// The <c>DnsAnalyticsService</c> handles communication with the DNS
    /// Analytics related methods of the DNSimple API.
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/dns-analytics/</see>
    public class DnsAnalyticsService : ServiceBase
    {
        /// <inheritdoc cref="ServiceBase"/>
        public DnsAnalyticsService(IClient client) : base(client)
        {
        }

        /// <summary>
        /// Queries the DNS Analytics data for the account.
        /// </summary>
        /// <remarks>This API is in Public Beta.</remarks>
        /// <param name="accountId">The account ID</param>
        /// <param name="options">Options passed to the query (sorting,
        /// filtering, grouping, pagination)</param>
        /// <returns>A <c>DnsAnalyticsResponse</c> containing the DNS
        /// Analytics data for the account.</returns>
        /// <see>https://developer.dnsimple.com/v2/dns-analytics/#queryDnsAnalytics</see>
        public DnsAnalyticsResponse Query(long accountId, DnsAnalyticsOptions? options = null)
        {
            var builder = BuildRequestForPath(DnsAnalyticsPath(accountId));
            AddListOptionsToRequest(options, ref builder);

            return new DnsAnalyticsResponse(Execute(builder.Request));
        }
    }

    /// <summary>
    /// Represents a response from a DNS Analytics query.
    /// </summary>
    /// <see cref="Pagination"/>
    public class DnsAnalyticsResponse : Response
    {
        /// <summary>
        /// The DNS Analytics data, one entry for each row.
        /// </summary>
        public List<DnsAnalytics> Data { get; }

        /// <summary>
        /// The <c>Pagination</c> object containing the pagination data
        /// </summary>
        /// <see cref="Pagination"/>
        public Pagination Pagination { get; }

        /// <summary>
        /// The query parameters that produce the data.
        /// </summary>
        public DnsAnalyticsQuery Query { get; }

        public DnsAnalyticsResponse(RestResponse response) : base(response)
        {
            var json = response.ParseContent();
            var headers = JsonTools<List<string>>.DeserializeObject("data.headers", json);
            var rows = JsonTools<List<JArray>>.DeserializeObject("data.rows", json);

            Data = rows.Select(row =>
                new JObject(headers.Select((header, index) =>
                    new JProperty(header, row[index]))).ToObject<DnsAnalytics>()).ToList();
            Pagination = Pagination.From(json);
            Query = JsonTools<DnsAnalyticsQuery>.DeserializeObject("query", json);
        }
    }

    /// <summary>
    /// Represents one row of DNS Analytics data.
    /// </summary>
    /// <remarks>The groupings of the query set which values are present.</remarks>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct DnsAnalytics
    {
        public string ZoneName { get; set; }
        public string Date { get; set; }
        public long? Volume { get; set; }
    }

    /// <summary>
    /// Represents the query parameters of a DNS Analytics query.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct DnsAnalyticsQuery
    {
        public long AccountId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Sort { get; set; }
        public int Page { get; set; }
        public int PerPage { get; set; }
        public string Groupings { get; set; }
    }
}
