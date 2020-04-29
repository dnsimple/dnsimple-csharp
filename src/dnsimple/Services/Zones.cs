using System;
using System.Collections.Generic;
using dnsimple.Services.ListOptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using static dnsimple.Services.JsonTools<dnsimple.Services.ZoneData>;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <summary>
    /// The <c>ZonesService</c> handles communication with the zone related
    /// methods of the DNSimple API.
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/zones/</see>
    public partial class ZonesService : Service
    {

        /// <inheritdoc cref="Service"/>
        public ZonesService(IClient client) : base(client) {}

        /// <summary>
        /// Lists the zones in the account.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <returns>A <c>ZonesResponse</c> containing a list of zones for the
        /// account.</returns>
        /// <see>https://developer.dnsimple.com/v2/zones/#listZones</see>
        public PaginatedDnsimpleResponse<ZonesData> ListZones(long accountId)
        {
            return new PaginatedDnsimpleResponse<ZonesData>(Client.Http.Execute(Client.Http
                .RequestBuilder(ZonesPath(accountId)).Request));
        }

        /// <summary>
        /// Lists the zones in the account.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="options">Options passed to the list (sorting,
        /// filtering, pagination)</param>
        /// <returns>A <c>ZonesResponse</c> containing a list of zones for
        /// the account.</returns>
        /// <see>https://developer.dnsimple.com/v2/zones/#listZones</see>
        public PaginatedDnsimpleResponse<ZonesData> ListZones(long accountId, ZonesListOptions options)
        {
            var requestBuilder = Client.Http
                .RequestBuilder(ZonesPath(accountId));
            
            requestBuilder.AddParameter(options.UnpackSorting());
            requestBuilder.AddParameters(options.UnpackFilters());
            
            if (!options.Pagination.IsDefault())
            {
                requestBuilder.AddParameters(options.UnpackPagination());
            }
            
            return new PaginatedDnsimpleResponse<ZonesData>(Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Retrieves a zone.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="zoneName">The zone name</param>
        /// <returns>A <c>ZoneResponse</c> containing the zone.</returns>
        /// <see>https://developer.dnsimple.com/v2/zones/#getZone</see>
        public SimpleDnsimpleResponse<ZoneData> GetZone(long accountId, string zoneName)
        {
            return new SimpleDnsimpleResponse<ZoneData>(Client.Http.Execute(Client.Http
                .RequestBuilder(ZonePath(accountId, zoneName)).Request));
        }

        /// <summary>
        /// Retrieves a zone file.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="zoneName">The zone name</param>
        /// <returns>A <c>ZoneFileResponse</c> containing the zone file content.</returns>
        /// <see>https://developer.dnsimple.com/v2/zones/#getZoneFile</see>
        public SimpleDnsimpleResponse<ZoneFile> GetZoneFile(long accountId, string zoneName)
        {
            return new SimpleDnsimpleResponse<ZoneFile>(Client.Http.Execute(Client.Http
                .RequestBuilder(ZoneFilePath(accountId, zoneName)).Request));
        }

        /// <summary>
        /// Checks if a zone change is fully distributed to all our nameservers
        /// across the globe.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="zoneName">The zone name</param>
        /// <returns>A <c>ZoneDistributionResponse</c>.</returns>
        /// <see>https://developer.dnsimple.com/v2/zones/#checkZoneDistribution</see>
        public SimpleDnsimpleResponse<ZoneDistribution> CheckZoneDistribution(long accountId,
            string zoneName)
        {
            return new SimpleDnsimpleResponse<ZoneDistribution>(Client.Http.Execute(Client.Http
                .RequestBuilder(ZoneDistributionPath(accountId, zoneName))
                .Request));
        }
    }

    /// <summary>
    /// Represents the <c>struct</c> containing a <c>List</c> of <c>ZoneData</c>
    /// objects.
    /// </summary>
    /// <see cref="List{T}"/>
    /// <see cref="ZoneData"/>
    public readonly struct ZonesData
    {
        /// <summary>
        /// The list of zones.
        /// </summary>
        public List<ZoneData> Zones { get; }

        /// <summary>
        /// Creates a new <c>ZonesData</c> object.
        /// </summary>
        /// <param name="json">The json payload containing the raw data.</param>
        public ZonesData(JToken json) =>
            Zones = DeserializeList(json);
    }

    /// <summary>
    /// Represents a zone.
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/zones/#zones-attributes</see>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct ZoneData
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public string Name { get; set; }
        public bool Reverse { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Represents a zone file.
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/zones/#zones-attributes</see>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct ZoneFile
    {
        public string Zone { get; set; }
    }

    /// <summary>
    /// Represents a zone distribution.
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/zones/#zones-attributes</see>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct ZoneDistribution
    {
        public bool Distributed { get; set; }
    }
}