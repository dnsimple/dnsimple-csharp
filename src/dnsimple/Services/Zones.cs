using System;
using dnsimple.Services.ListOptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <summary>
    /// The <c>ZonesService</c> handles communication with the zone related
    /// methods of the DNSimple API.
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/zones/</see>
    public partial class ZonesService : ServiceBase
    {
        /// <inheritdoc cref="ServiceBase"/>
        public ZonesService(IClient client) : base(client)
        {
        }

        /// <summary>
        /// Lists the zones in the account.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="options">Options passed to the list (sorting,
        /// filtering, pagination)</param>
        /// <returns>A <c>ZonesResponse</c> containing a list of zones for
        /// the account.</returns>
        /// <see>https://developer.dnsimple.com/v2/zones/#listZones</see>
        public PaginatedResponse<Zone> ListZones(long accountId, ZonesListOptions options = null)
        {
            var builder = BuildRequestForPath(ZonesPath(accountId));
            AddListOptionsToRequest(options, ref builder);

            return new PaginatedResponse<Zone>(Execute(builder.Request));
        }

        /// <summary>
        /// Retrieves the details of an existing zone.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="zoneName">The zone name</param>
        /// <returns>A <c>ZoneResponse</c> containing the zone.</returns>
        /// <see>https://developer.dnsimple.com/v2/zones/#getZone</see>
        public SimpleResponse<Zone> GetZone(long accountId, string zoneName)
        {
            var builder = BuildRequestForPath(ZonePath(accountId, zoneName));
                
            return new SimpleResponse<Zone>(Execute(builder.Request));
        }

        /// <summary>
        /// Download the zonefile for an existing zone.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="zoneName">The zone name</param>
        /// <returns>A <c>ZoneFileResponse</c> containing the zone file content.</returns>
        /// <see>https://developer.dnsimple.com/v2/zones/#getZoneFile</see>
        public SimpleResponse<ZoneFile> GetZoneFile(long accountId, string zoneName)
        {
            var builder = BuildRequestForPath(ZoneFilePath(accountId, zoneName));

            return new SimpleResponse<ZoneFile>(Execute(builder.Request));
        }

        /// <summary>
        /// Checks if a zone is fully distributed to all our name servers across the globe.
        /// </summary>
        /// <remarks>This feature is not available for testing in our Sandbox environment.</remarks>
        /// <param name="accountId">The account ID</param>
        /// <param name="zoneName">The zone name</param>
        /// <returns>A <c>ZoneDistributionResponse</c>.</returns>
        /// <see>https://developer.dnsimple.com/v2/zones/#checkZoneDistribution</see>
        public SimpleResponse<ZoneDistribution> CheckZoneDistribution(long accountId, string zoneName)
        {
            var builder = BuildRequestForPath(ZoneDistributionPath(accountId, zoneName));

            return new SimpleResponse<ZoneDistribution>(Execute(builder.Request));
        }
    }

    /// <summary>
    /// Represents a zone.
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/zones/#zones-attributes</see>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct Zone
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
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct ZoneFile
    {
        public string Zone { get; set; }
    }

    /// <summary>
    /// Represents a zone distribution.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct ZoneDistribution
    {
        public bool Distributed { get; set; }
    }
}