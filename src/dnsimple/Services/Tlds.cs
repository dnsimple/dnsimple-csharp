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
        /// Returns the list of TLDs supported for registration or transfer.
        /// </summary>
        /// <returns>The list of TLDs supported</returns>
        /// <see cref="TldData"/>
        /// <param name="options">Options passed to the list (sorting and
        ///  pagination)</param>
        /// <see>https://developer.dnsimple.com/v2/tlds/#listTlds</see>
        public PaginatedDnsimpleResponse<TldData> ListTlds(
            TldListOptions options = null)
        {
            var builder = BuildRequestForPath(TldsPath());

            AddListOptionsToRequest(options, ref builder);

            return new PaginatedDnsimpleResponse<TldData>(
                Execute(builder.Request));
        }

        /// <summary>
        /// Retrieves the details of a supported TLD.
        /// </summary>
        /// <param name="tld">The TLD name</param>
        /// <returns>The information about the TLD requested</returns>
        /// <see cref="TldData"/>
        /// <see>https://developer.dnsimple.com/v2/tlds/#getTld</see>
        public SimpleDnsimpleResponse<TldData> GetTld(string tld)
        {
            return new SimpleDnsimpleResponse<TldData>(Execute(
                BuildRequestForPath(GetTldPath(tld)).Request));
        }

        /// <summary>
        /// Lists the TLD Extended Attributes
        /// </summary>
        /// <remarks>
        /// <para>
        /// Some TLDs require extended attributes when registering or
        /// transferring a domain. This API interface provides information on
        /// the extended attributes for any particular TLD.</para>
        /// <para>
        /// Extended attributes are extra TLD-specific attributes, required by
        /// the TLD registry to collect extra information about the registrant
        /// or legal agreements.
        /// </para>
        /// </remarks>
        /// <param name="tld">The TLD name</param>
        /// <returns>The extended attributes list for the TLD.</returns>
        /// <see cref="TldExtendedAttribute"/>
        /// <see>https://developer.dnsimple.com/v2/tlds/#getTldExtendedAttributes</see>
        public ListDnsimpleResponse<TldExtendedAttribute>
            GetTldExtendedAttributes(string tld)
        {
            return new ListDnsimpleResponse<TldExtendedAttribute>(
                Execute(BuildRequestForPath(GetTldExtendedAttributesPath(tld))
                    .Request));
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
    }
}