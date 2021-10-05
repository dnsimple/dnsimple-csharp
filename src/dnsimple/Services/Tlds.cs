using System.Collections.Generic;
using dnsimple.Services.ListOptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <inheritdoc cref="ServiceBase"/>
    /// <see>https://developer.dnsimple.com/v2/tlds/</see>
    public class TldsService : ServiceBase
    {
        /// <inheritdoc cref="ServiceBase"/>
        public TldsService(IClient client) : base(client)
        {
        }

        /// <summary>
        /// ListsTLDs supported for registration or transfer.
        /// </summary>
        /// <returns>The list of TLDs supported</returns>
        /// <see cref="TldData"/>
        /// <param name="options">Options passed to the list (sorting and
        ///  pagination)</param>
        /// <see>https://developer.dnsimple.com/v2/tlds/#listTlds</see>
        public PaginatedResponse<TldData> ListTlds(TldListOptions options = null)
        {
            var builder = BuildRequestForPath(TldsPath());
            AddListOptionsToRequest(options, ref builder);

            return new PaginatedResponse<TldData>(Execute(builder.Request));
        }

        /// <summary>
        /// Retrieves the details of a TLD.
        /// </summary>
        /// <param name="tld">The TLD name</param>
        /// <returns>The information about the TLD requested</returns>
        /// <see cref="TldData"/>
        /// <see>https://developer.dnsimple.com/v2/tlds/#getTld</see>
        public SimpleResponse<TldData> GetTld(string tld)
        {
            var builder = BuildRequestForPath(GetTldPath(tld));

            return new SimpleResponse<TldData>(Execute(builder.Request));
        }

        /// <summary>
        /// Lists a TLD extended attributes.
        /// </summary>
        /// <param name="tld">The TLD name</param>
        /// <returns>The extended attributes list for the TLD.</returns>
        /// <see cref="TldExtendedAttribute"/>
        /// <see>https://developer.dnsimple.com/v2/tlds/#getTldExtendedAttributes</see>
        public ListResponse<TldExtendedAttribute> GetTldExtendedAttributes(string tld)
        {
            var builder = BuildRequestForPath(GetTldExtendedAttributesPath(tld));

            return new ListResponse<TldExtendedAttribute>(Execute(builder.Request));
        }
    }

    /// <summary>
    /// Represents a TLDs extended attribute
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct TldExtendedAttribute
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? Required { get; set; }
        public List<TldExtendedAttributeOption> Options { get; set; }
    }

    /// <summary>
    /// Represents an TLDs extended attribute option
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct TldExtendedAttributeOption
    {
        public string Title { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// Represents a TLD.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct TldData
    {
        public string Tld { get; set; }
        public long TldType { get; set; }
        public bool? WhoisPrivacy { get; set; }
        public bool? AutoRenewOnly { get; set; }
        public bool? Idn { get; set; }
        public long MinimumRegistration { get; set; }
        public bool? RegistrationEnabled { get; set; }
        public bool? RenewalEnabled { get; set; }
        public bool? TransferEnabled { get; set; }
        public string DnssecInterfaceType { get; set; }
    }
}
