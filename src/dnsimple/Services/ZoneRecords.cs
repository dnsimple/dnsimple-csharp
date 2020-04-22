using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.JsonTools<dnsimple.Services.ZoneRecordData>;

namespace dnsimple.Services
{
    /// <inheritdoc cref="ZonesService"/>
    public partial class ZonesService
    {
        /// <summary>
        /// Lists the zone records.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="zoneId">The zone name</param>
        /// <returns>A <c>ZoneRecordsResponse</c> containing a list of zone
        /// records for the zone.</returns>
        /// <see cref="ZoneRecordsResponse"/>
        /// <see>https://developer.dnsimple.com/v2/zones/records/#listZoneRecords</see>
        public ZoneRecordsResponse ListRecords(long accountId, string zoneId)
        {
            return new ZoneRecordsResponse(Client.Http.Execute(Client.Http
                .RequestBuilder(ZoneRecordsPath(accountId, zoneId)).Request));
        }

        /// <summary>
        /// Lists the zone records.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="zoneId">The zone name</param>
        /// <param name="options">Options passed to the list (sorting,
        /// filtering, pagination)</param>
        /// <returns>A <c>ZoneRecordsResponse</c> containing a list of zone
        /// records for the zone.</returns>
        /// <see cref="ZoneRecordsResponse"/>
        /// <see cref="ZoneRecordsListOptions"/>
        /// <see>https://developer.dnsimple.com/v2/zones/records/#listZoneRecords</see>
        public ZoneRecordsResponse ListRecords(long accountId, string zoneId,
            ZoneRecordsListOptions options)
        {
            var requestBuilder = Client.Http
                .RequestBuilder(ZoneRecordsPath(accountId, zoneId));

            requestBuilder.AddParameters(options.UnpackFilters());
            requestBuilder.AddParameters(options.UnpackFilters());

            if (!options.Pagination.IsDefault())
            {
                requestBuilder.AddParameters(options.UnpackPagination());
            }

            return new ZoneRecordsResponse(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Creates a new zone record.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="zoneId">The zone name</param>
        /// <param name="record">The zone record input</param>
        /// <returns>The newly created <c>ZoneRecord</c> wrapped inside a
        /// <c>ZoneRecordResponse</c></returns>
        /// <see cref="ZoneRecordResponse"/>
        /// <see cref="ZoneRecord"/>
        /// <see>https://developer.dnsimple.com/v2/zones/records/#createZoneRecord</see>
        public ZoneRecordResponse CreateRecord(long accountId, string zoneId,
            ZoneRecord record)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(ZoneRecordsPath(accountId, zoneId));
            requestBuilder.Method(Method.POST);
            requestBuilder.AddJsonPayload(record);

            return new ZoneRecordResponse(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Retrieves a zone record.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="zoneId">The zone name</param>
        /// <param name="recordId">The record id</param>
        /// <returns>A <c>ZoneRecordResponse</c> containing the zone record.</returns>
        /// <see cref="ZoneRecordResponse"/>
        /// <see cref="ZoneRecord"/>
        /// <see>https://developer.dnsimple.com/v2/zones/records/#getZoneRecord</see>
        public ZoneRecordResponse GetRecord(long accountId, string zoneId,
            long recordId)
        {
            return new ZoneRecordResponse(Client.Http.Execute(Client.Http
                .RequestBuilder(ZoneRecordPath(accountId, zoneId, recordId))
                .Request));
        }

        /// <summary>
        /// Updates an existing zone record.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="zoneId">The record name</param>
        /// <param name="recordId">The record id</param>
        /// <param name="record">The zone record input</param>
        /// <returns>The newly updated <c>ZoneRecord</c> wrapped inside a
        /// <c>ZoneRecordResponse</c></returns>
        /// <see cref="ZoneRecordResponse"/>
        /// <see cref="ZoneRecord"/>
        /// <see>https://developer.dnsimple.com/v2/zones/records/#updateZoneRecord</see>
        public ZoneRecordResponse UpdateRecord(long accountId, string zoneId, long recordId, ZoneRecord record)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(ZoneRecordPath(accountId, zoneId, recordId));
            requestBuilder.Method(Method.PATCH);
            requestBuilder.AddJsonPayload(recordId);

            return new ZoneRecordResponse(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Permanently deletes a zone record.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="zoneId">The zone name</param>
        /// <param name="recordId">The record Id</param>
        /// <see>https://developer.dnsimple.com/v2/zones/records/#deleteZoneRecord</see>
        public void DeleteRecord(long accountId, string zoneId, long recordId)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(ZoneRecordPath(accountId, zoneId,
                    recordId));
            requestBuilder.Method(Method.DELETE);

            Client.Http.Execute(requestBuilder.Request);
        }

        /// <summary>
        /// Checks if a zone change is fully distributed to all our nameservers
        /// across the globe.
        /// </summary>
        /// <remarks>This feature is not available for testing in our Sandbox environment.</remarks>
        /// <param name="accountId">The account Id</param>
        /// <param name="zoneId">The zone name</param>
        /// <param name="recordId">The record id</param>
        /// <returns>A <c>ZoneDistributionResponse</c>.</returns>
        /// <see cref="ZoneDistributionResponse"/>
        /// <see>https://developer.dnsimple.com/v2/zones/records/#checkZoneRecordDistribution</see>
        public ZoneDistributionResponse CheckRecordDistribution(long accountId, string zoneId, long recordId)
        {
            return new ZoneDistributionResponse(Client.Http.Execute(Client.Http
                .RequestBuilder(ZoneRecordDistributionPath(accountId, zoneId, recordId))
                .Request));
        }

        private static string ZoneRecordDistributionPath(long accountId,
            string zoneId, long recordId)
        {
            return
                $"{ZoneRecordPath(accountId, zoneId, recordId)}/distribution";
        }

        private static string ZoneRecordPath(long accountId, string zoneId,
            long recordId)
        {
            return $"{ZoneRecordsPath(accountId, zoneId)}/{recordId}";
        }

        private static string ZoneRecordsPath(long accountId, string zoneId)
        {
            return $"{ZonePath(accountId, zoneId)}/records";
        }
    }
    
    /// <summary>
    /// Represents the response from the API call containing the
    /// <c>ZoneRecordResponse</c>.
    /// </summary>
    /// <see cref="ZoneRecordData"/>
    public class ZoneRecordResponse : SimpleDnsimpleResponse<ZoneRecordData>
    {
        public ZoneRecordResponse(JToken json) : base(json)
        {
        }
    }

    /// <summary>
    /// Represents the response from the API call containing the
    /// <c>ZoneRecordsResponse</c> with (potentially) multiple <c>ZoneRecordData</c>
    /// objects and a <c>PaginationData</c> object.
    /// </summary>
    /// <see cref="ZoneRecordsData"/>
    /// <see cref="PaginationData"/>
    public class ZoneRecordsResponse : PaginatedDnsimpleResponse<ZoneRecordsData>
    {
        public ZoneRecordsResponse(JToken response) : base(response) =>
            Data = new ZoneRecordsData(response);
    }

    /// <summary>
    /// Represents the struct containing a <c>List</c> of <c>ZoneRecordData</c>
    /// objects.
    /// </summary>
    /// <see cref="List{T}"/>
    /// <see cref="ZoneRecordData"/>
    public readonly struct ZoneRecordsData
    {
        public List<ZoneRecordData> Records { get; }

        public ZoneRecordsData(JToken json) =>
            Records = DeserializeList(json);
    }

    /// <summary>
    /// Represents a zone Record
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct ZoneRecordData
    {
        public long Id { get; set; }
        public string ZoneId { get; set; }
        public long? ParentId { get; set; }
        public string Name { get; set; }
        public string Content { get; set; }
        public long Ttl { get; set; }
        public long? Priority { get; set; }
        public ZoneRecordType Type { get; set; }
        public List<string> Regions { get; set; }
        public bool SystemRecord { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Represents a zone record type.
    /// </summary>
    public enum ZoneRecordType
    {
        A,
        AAAA,
        ALIAS,
        CAA,
        CNAME,
        DNSKEY,
        DS,
        HINFO,
        MX,
        NAPTR,
        NS,
        POOL,
        PTR,
        SOA,
        SPF,
        SRV,
        SSHFP,
        TXT,
        URL
    }

    /// <summary>
    /// Represents a zone record
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public class ZoneRecord
    {
        public string Name { get; set; }
        public ZoneRecordType Type { get; set; }
        public string Content { get; set; }
        public long Ttl { get; set; }
        public long? Priority { get; set; }
        public List<string> Regions { get; private set; } = new List<string>();

        /// <summary>
        /// Replaces the regions list with a new list of regions.
        /// </summary>
        /// <param name="regions">The regions list we want</param>
        /// <see cref="List{T}"/>
        /// <see cref="Region"/>
        public void AddRegions(List<Region> regions)
        {
            Regions = regions.Select(region => region.ToString()).ToList();
        }

        /// <summary>
        /// Adds a region to the regions list.
        /// </summary>
        /// <param name="region">The region we want to add</param>
        /// <see cref="Region"/>
        public void AddRegion(Region region)
        {
            Regions.Add(region.ToString());
        }
    }

    /// <summary>
    /// Represents a Region.
    ///
    /// Zone Record Regions lets you select geographical regions where you want
    /// a record to appear.
    /// </summary>
    /// <remarks>The zone record regions is a feature that is only available to
    /// the following new plans: Professional and Business. If the feature is
    /// not enabled, you will receive an HTTP 412 response code.</remarks>
    public enum Region
    {
        SV1,
        ORD,
        IAD,
        AMS,
        TKO,
        SYD
    }

    /// <summary>
    /// Defines the options you may want to send to list zone records, such as
    /// pagination, sorting and filtering.
    /// </summary>
    /// <see cref="ListOptionsWithFiltering"/>
    public class ZoneRecordsListOptions : ListOptionsWithFiltering
    {
        private const string NameLikeFilter = "name_like";
        private const string NameExactFilter = "name";
        private const string TypeFilter = "type";

        private const string IdSort = "id";
        private const string NameSort = "name";
        private const string ContentSort = "content";
        private const string TypeSort = "type";

        /// <summary>
        /// Creates a new instance of <c>ZoneRecordsListOptions</c>.
        /// </summary>
        public ZoneRecordsListOptions() =>
            Pagination = new Pagination();

        /// <summary>
        /// Only include records containing given string.
        /// </summary>
        /// <param name="name">The name we want to filter by</param>
        /// <returns>The instance of the <c>ZoneRecordsListOptions</c></returns>
        public ZoneRecordsListOptions FilterByName(string name)
        {
            AddFilter(new Filter {Field = NameLikeFilter, Value = name});
            return this;
        }

        /// <summary>
        /// Only include records with name equal to given string.
        /// </summary>
        /// <param name="name">The name we want to filter by</param>
        /// <returns>The instance of the <c>ZoneRecordsListOptions</c></returns>
        public ZoneRecordsListOptions FilterByExactName(string name)
        {
            AddFilter(new Filter {Field = NameExactFilter, Value = name});
            return this;
        }
        
        /// <summary>
        /// Only include records with record type equal to given string
        /// </summary>
        /// <param name="type">The record type we want to filter by</param>
        /// <returns>The instance of the <c>ZoneRecordsListOptions</c></returns>
        public ZoneRecordsListOptions FilterByType(ZoneRecordType type)
        {
            AddFilter(new Filter {Field = TypeFilter, Value = type.ToString()});
            return this;
        }

        /// <summary>
        /// Sort records by ID.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>ZoneRecordsListOptions</c></returns>
        /// <see cref="Order"/>
        public ZoneRecordsListOptions SortById(Order order)
        {
            AddSortCriteria(new Sort {Field = IdSort, Order = order});
            return this;
        }

        /// <summary>
        /// Sort records by name (alphabetical order).
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>ZoneRecordsListOptions</c></returns>
        /// <see cref="Order"/>
        public ZoneRecordsListOptions SortByName(Order order)
        {
            AddSortCriteria(new Sort {Field = NameSort, Order = order});
            return this;
        }

        /// <summary>
        /// Sort records by content.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>ZoneRecordsListOptions</c></returns>
        /// <see cref="Order"/>
        public ZoneRecordsListOptions SortByContent(Order order)
        {
            AddSortCriteria(new Sort {Field = ContentSort, Order = order});
            return this;
        }

        /// <summary>
        /// Sort records by type.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>ZoneRecordsListOptions</c></returns>
        /// <see cref="Order"/>
        public ZoneRecordsListOptions SortByType(Order order)
        {
            AddSortCriteria(new Sort {Field = TypeSort, Order = order});
            return this;
        }
    }
}