using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using static dnsimple.JsonTools<dnsimple.Services.ZoneData>;

namespace dnsimple.Services
{
    /// <summary>
    /// The <c>ZonesService</c> handles communication with the zone related
    /// methods of the DNSimple API
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/zones/</see>
    public class ZonesService
    {
        private IClient Client { get; }

        /// <summary>
        /// Creates a new instance of a <c>ZonesService</c> by passing an
        /// instance of the DNSimple <c>IClient</c>
        /// </summary>
        /// <param name="client">An instance of the <c>IClient</c></param>
        /// <see cref="IClient"/>
        public ZonesService(IClient client) =>
            Client = client;

        /// <summary>
        /// Lists the zones in the account.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <returns>A <c>ZonesResponse</c> containing a list of zones for the account.</returns>
        /// <see cref="ZonesResponse"/>
        /// <see>https://developer.dnsimple.com/v2/zones/#listZones</see>
        public ZonesResponse ListZones(long accountId)
        {
            return new ZonesResponse(Client.Http.Execute(Client.Http
                .RequestBuilder(ZonesPath(accountId)).Request));
        }

        /// <summary>
        /// Lists the zones in the account.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="options">Options passed to the list (sorting, filtering, pagination)</param>
        /// <returns>A <c>ZonesResponse</c> containing a list of zones for the account.</returns>
        /// <see cref="ZonesResponse"/>
        /// <see>https://developer.dnsimple.com/v2/zones/#listZones</see>
        public ZonesResponse ListZones(long accountId, ZonesListOptions options)
        {
            var requestBuilder = Client.Http
                .RequestBuilder(ZonesPath(accountId));
            
            requestBuilder.AddParameter(options.UnpackSorting());
            requestBuilder.AddParameters(options.UnpackFilters());
            
            if (!options.Pagination.IsDefault())
            {
                requestBuilder.AddParameters(options.UnpackPagination());
            }
            
            return new ZonesResponse(Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Retrieves a zone.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="zoneName">The zone name</param>
        /// <returns>A <c>ZoneResponse</c> containing the zone.</returns>
        /// <see cref="ZoneResponse"/>
        /// <see>https://developer.dnsimple.com/v2/zones/#getZone</see>
        public ZoneResponse GetZone(long accountId, string zoneName)
        {
            return new ZoneResponse(Client.Http.Execute(Client.Http
                .RequestBuilder(ZonePath(accountId, zoneName)).Request));
        }

        /// <summary>
        /// Retrieves a zone file.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="zoneName">The zone name</param>
        /// <returns>A <c>ZoneFileResponse</c> containing the zone file content.</returns>
        /// <see>https://developer.dnsimple.com/v2/zones/#getZoneFile</see>
        public ZoneFileResponse GetZoneFile(long accountId, string zoneName)
        {
            return new ZoneFileResponse(Client.Http.Execute(Client.Http
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
        public ZoneDistributionResponse CheckZoneDistribution(long accountId,
            string zoneName)
        {
            return new ZoneDistributionResponse(Client.Http.Execute(Client.Http
                .RequestBuilder(ZoneDistributionPath(accountId, zoneName))
                .Request));
        }

        private static string ZoneDistributionPath(long accountId,
            string zoneName)
        {
            return $"{ZonePath(accountId, zoneName)}/distribution";
        }

        private static string ZoneFilePath(long accountId, string zoneName)
        {
            return $"{ZonePath(accountId, zoneName)}/file";
        }

        private static string ZonePath(long accountId, string zoneName)
        {
            return $"{ZonesPath(accountId)}/{zoneName}";
        }

        private static string ZonesPath(long accountId)
        {
            return $"/{accountId}/zones";
        }
    }

    /// <summary>
    /// Represents the response from the API call containing the <c>ZoneDistribution</c>.
    /// </summary>
    /// <see cref="ZoneDistribution"/>
    public class ZoneDistributionResponse : SimpleDnsimpleResponse<ZoneDistribution>
    {
        public ZoneDistributionResponse(JToken json) : base(json)
        {
        }
    }

    /// <summary>
    /// Represents the response from the API call containing the <c>ZoneFile</c>.
    /// </summary>
    /// <see cref="ZoneFile"/>
    public class ZoneFileResponse : SimpleDnsimpleResponse<ZoneFile>
    {
        public ZoneFileResponse(JToken json) : base(json)
        {
        }
    }

    /// <summary>
    /// Represents the response from the API call containing the <c>ZoneData</c>.
    /// </summary>
    /// <see cref="ZoneData"/>
    public class ZoneResponse : SimpleDnsimpleResponse<ZoneData>
    {
        public ZoneResponse(JToken json) : base(json)
        {
        }
    }

    /// <summary>
    /// Represents the response from the API call containing (potentially)
    /// multiple <c>ZonesData</c> objects and a <c>Pagination</c> object.
    /// </summary>
    /// <see cref="ZonesData"/>
    /// <see cref="PaginationData"/>
    public class ZonesResponse : PaginatedDnsimpleResponse<ZonesData>
    {
        public ZonesResponse(JToken response) : base(response) =>
            Data = new ZonesData(response);
    }

    /// <summary>
    /// Represents the <c>struct</c> containing a <c>List</c> of <c>ZoneData</c>
    /// objects.
    /// </summary>
    /// <see cref="List{T}"/>
    /// <see cref="ZoneData"/>
    public readonly struct ZonesData
    {
        public List<ZoneData> Zones { get; }

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
    
    /// <summary>
    /// Defines the options you may want to send to list zones, such as
    /// pagination, sorting and filtering.
    /// </summary>
    /// <see cref="ListOptionsWithFiltering"/>
    public class ZonesListOptions : ListOptionsWithFiltering
    {
        private const string NameLikeFilter = "name_like";
        
        private const string IdSort = "id";
        private const string NameSort = "name";

        /// <summary>
        /// Creates a new instance of <c>ZonesListOptions</c>.
        /// </summary>
        public ZonesListOptions()
            => Pagination = new Pagination();
        
        /// <summary>
        /// Sets the name to be filtered by.
        /// </summary>
        /// <param name="name">The name we want to filter by.</param>
        /// <returns>The instance of the <c>ZonesListOptions</c></returns>
        public ZonesListOptions FilterByName(string name)
        {
            AddFilter(new Filter { Field = NameLikeFilter, Value = name });
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by id.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>ZonesListOptions</c></returns>
        public ZonesListOptions SortById(Order order)
        {
            AddSortCriteria(new Sort { Field = IdSort, Order = order });
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by name.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>ZonesListOptions</c></returns>
        public ZonesListOptions SortByName(Order order)
        {
            AddSortCriteria(new Sort { Field = NameSort, Order = order});
            return this;
        }
    }
}