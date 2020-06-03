using System;
using System.Collections.Generic;
using System.Linq;
using dnsimple.Services.ListOptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <inheritdoc cref="ZonesService"/>
    public partial class ZonesService
    {
        /// <summary>
        /// Lists the records for a zone.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="zoneId">The zone name</param>
        /// <param name="options">Options passed to the list (sorting,
        /// filtering, pagination)</param>
        /// <returns>A <c>ZoneRecordsResponse</c> containing a list of zone
        /// records for the zone.</returns>
        /// <see cref="ZoneRecordsListOptions"/>
        /// <see>https://developer.dnsimple.com/v2/zones/records/#listZoneRecords</see>
        public PaginatedResponse<ZoneRecord> ListZoneRecords(long accountId, string zoneId, ZoneRecordsListOptions options = null)
        {
            var builder = BuildRequestForPath(ZoneRecordsPath(accountId, zoneId));

            AddListOptionsToRequest(options, ref builder);

            return new PaginatedResponse<ZoneRecord>(Execute(builder.Request));
        }

        /// <summary>
        /// Creates a new zone record.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="zoneId">The zone name</param>
        /// <param name="input">The zone record input</param>
        /// <returns>The newly created <c>ZoneRecord</c> wrapped inside a
        /// <c>ZoneRecordResponse</c></returns>
        /// <exception cref="DnsimpleException">If Bad Request</exception>
        /// <exception cref="DnsimpleValidationException">If the validation fails</exception>
        /// <see cref="ZoneRecord"/>
        /// <see>https://developer.dnsimple.com/v2/zones/records/#createZoneRecord</see>
        public SimpleResponse<ZoneRecord> CreateZoneRecord(long accountId, string zoneId, ZoneRecord input)
        {
            var builder = BuildRequestForPath(ZoneRecordsPath(accountId, zoneId));
            builder.Method(Method.POST);
            builder.AddJsonPayload(PrepareRecord(input));

            return new SimpleResponse<ZoneRecord>(Execute(builder.Request));
        }

        private static ZoneRecordToSend PrepareRecord(ZoneRecord record)
        {
            var newRecord = new ZoneRecordToSend(record);
            return newRecord;
        }

        /// <summary>
        /// Retrieves the details of an existing zone record.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="zoneId">The zone name</param>
        /// <param name="recordId">The record id</param>
        /// <returns>A <c>ZoneRecordResponse</c> containing the zone record.</returns>
        /// <see cref="ZoneRecord"/>
        /// <see>https://developer.dnsimple.com/v2/zones/records/#getZoneRecord</see>
        public SimpleResponse<ZoneRecord> GetZoneRecord(long accountId, string zoneId, long recordId)
        {
            var builder = BuildRequestForPath(ZoneRecordPath(accountId, zoneId, recordId));
                
            return new SimpleResponse<ZoneRecord>(Execute(builder.Request));
        }

        /// <summary>
        /// Updates the zone record details.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="zoneId">The zone name</param>
        /// <param name="recordId">The record id</param>
        /// <param name="record">The zone record input</param>
        /// <returns>The newly updated <c>ZoneRecord</c> wrapped inside a
        /// <c>ZoneRecordResponse</c></returns>
        /// <exception cref="DnsimpleException">If bad request</exception>
        /// <exception cref="DnsimpleValidationException">If the validation fails</exception>
        /// <see cref="ZoneRecord"/>
        /// <see>https://developer.dnsimple.com/v2/zones/records/#updateZoneRecord</see>
        public SimpleResponse<ZoneRecord> UpdateZoneRecord(long accountId, string zoneId, long recordId, ZoneRecord record)
        {
            var builder = BuildRequestForPath(ZoneRecordPath(accountId, zoneId, recordId));
            builder.Method(Method.PATCH);
            builder.AddJsonPayload(record);

            return new SimpleResponse<ZoneRecord>(Execute(builder.Request));
        }

        /// <summary>
        /// Permanently deletes a zone record.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="zoneId">The zone name</param>
        /// <param name="recordId">The record Id</param>
        /// <see>https://developer.dnsimple.com/v2/zones/records/#deleteZoneRecord</see>
        public EmptyResponse DeleteZoneRecord(long accountId, string zoneId, long recordId)
        {
            var builder = BuildRequestForPath(ZoneRecordPath(accountId, zoneId, recordId));
            builder.Method(Method.DELETE);

            return new EmptyResponse(Execute(builder.Request));
        }

        /// <summary>
        /// Checks if a zone change is fully distributed to all our name servers across the globe.
        /// </summary>
        /// <remarks>This feature is not available for testing in our Sandbox environment.</remarks>
        /// <param name="accountId">The account ID</param>
        /// <param name="zoneId">The zone name</param>
        /// <param name="recordId">The record id</param>
        /// <returns>A <c>ZoneDistributionResponse</c>.</returns>
        /// <see>https://developer.dnsimple.com/v2/zones/records/#checkZoneRecordDistribution</see>
        public SimpleResponse<ZoneDistribution> CheckRecordDistribution( long accountId, string zoneId, long recordId)
        {
            var builder = BuildRequestForPath(ZoneRecordDistributionPath(accountId, zoneId, recordId)); 

            return new SimpleResponse<ZoneDistribution>(Execute(builder.Request));
        }
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
    /// Represents a zone Record
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy),ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct ZoneRecord
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

    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    internal class ZoneRecordToSend
    {
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Type { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Content { get; set; }

        public long Ttl { get; set; }
        public long? Priority { get; set; }
        public List<string> Regions { get; set; }

        internal ZoneRecordToSend(ZoneRecord record)
        {
            Name = record.Name;
            Type = record.Type.ToString();
            Content = record.Content;
            Ttl = record.Ttl;
            Priority = record.Priority;
            if (record.Regions != null && record.Regions.Count > 0) {
                Regions = record.Regions.Select(region => region.ToString()).ToList();
            }
        }
    }
}
