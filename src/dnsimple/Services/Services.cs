using System;
using System.Collections.Generic;
using dnsimple.Services.ListOptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <summary>
    /// Handles communication with the service related methods of the
    /// DNSimple API.
    /// </summary>
    /// <see cref="ServiceBase"/>
    public partial class ServicesService : ServiceBase
    {
        /// <inheritdoc cref="ServiceBase"/>
        public ServicesService(IClient client) : base(client)
        {
        }

        /// <summary>
        /// List all available one-click services.
        /// </summary>
        /// <param name="options">Options passed to the list (sorting and
        /// pagination).</param>
        /// <returns>A list of all the one-click services available</returns>
        /// <see>https://developer.dnsimple.com/v2/services/#listServices</see>
        public PaginatedResponse<Service> ListServices(ListServicesOptions options = null)
        {
            var builder = BuildRequestForPath(ServicesPath());
            AddListOptionsToRequest(options, ref builder);

            return new PaginatedResponse<Service>(Execute(builder.Request));
        }

        /// <summary>
        /// Retrieves the details of a one-click service.
        /// </summary>
        /// <param name="serviceIdentifier">The service name or ID</param>
        /// <returns>The one-click service requested.</returns>
        /// <see>https://developer.dnsimple.com/v2/services/#getService</see>
        public SimpleResponse<Service> GetService(string serviceIdentifier)
        {
            var builder = BuildRequestForPath(ServicePath(serviceIdentifier));

            return new SimpleResponse<Service>(Execute(builder.Request));
        }
    }

    /// <summary>
    /// Represents a one-click service.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct Service
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Sid { get; set; }
        public string Description { get; set; }
        public string SetupDescription { get; set; }
        public bool? RequiresSetup { get; set; }
        public string DefaultSubdomain { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public IList<ServiceSetting> Settings { get; set; }
    }

    /// <summary>
    /// Represents a single group of settings for a DNSimple Service.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct ServiceSetting
    {
        public string Name { get; set; }
        public string Label { get; set; }
        public string Append { get; set; }
        public string Description { get; set; }
        public string Example { get; set; }
        public bool? Password { get; set; }
    }
}
