using System;
using dnsimple.Services.ListOptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <inheritdoc cref="DomainsService"/>
    /// <see>https://developer.dnsimple.com/v2/domains/dnssec/</see>
    public partial class DomainsService
    {
        /// <summary>
        /// List delegation signer records for the domain in the account.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <param name="options">Options passed to the list (sorting, pagination)</param>
        /// <returns>A list of delegation signer records wrapped in a response</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/dnssec/#listDomainDelegationSignerRecords</see>
        public PaginatedResponse<DelegationSignerRecord>
            ListDelegationSignerRecords(
                long accountId, string domainIdentifier,
                ListDomainDelegationSignerRecordsOptions options = null)
        {
            var builder = BuildRequestForPath(DsRecordsPath(accountId,
                domainIdentifier));

            AddListOptionsToRequest(options, ref builder);

            return new PaginatedResponse<DelegationSignerRecord>(
                Execute(builder.Request));
        }

        /// <summary>
        /// You only need to create a delegation signer record manually if your
        /// domain is registered with DNSimple but hosted with another DNS
        /// provider that is signing your zone. To enable DNSSEC on a domain
        /// that is hosted with DNSimple, use the DNSSEC enable endpoint.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <param name="record">The delegation signer record to be added</param>
        /// <see cref="DelegationSignerRecord"/>
        /// <returns>The newly created delegation signer record.</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/dnssec/#createDomainDelegationSignerRecord</see>
        /// <see>"https://tools.ietf.org/html/rfc4034"</see>
        public SimpleResponse<DelegationSignerRecord>
            CreateDelegationSignerRecord(
                long accountId, string domainIdentifier,
                DelegationSignerRecord record)
        {
            var builder = BuildRequestForPath(DsRecordsPath(accountId,
                domainIdentifier));
            builder.Method(Method.POST);
            builder.AddJsonPayload(record);

            if (record.Algorithm.Trim().Equals("") ||
                record.Digest.Trim().Equals("") ||
                record.DigestType.Trim().Equals("") ||
                record.Keytag.Trim().Equals(""))
                throw new ArgumentException(
                    "Algorithm, Digest, DigestType and Keytag cannot be null or empty");

            return new SimpleResponse<DelegationSignerRecord>(
                Execute(builder.Request));
        }

        /// <summary>
        /// Retrieves a delegation signer record.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <param name="recordId">The delegation signer record id</param>
        /// <returns>The delegation signer record</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/dnssec/#getDomainDelegationSignerRecord</see>
        public SimpleResponse<DelegationSignerRecord>
            GetDelegationSignerRecord(
                long accountId, string domainIdentifier, long recordId)
        {
            return new SimpleResponse<DelegationSignerRecord>(
                Execute(BuildRequestForPath(DsRecordPath(accountId,
                    domainIdentifier,
                    recordId)).Request));
        }

        /// <summary>
        /// Deletes a delegation signer record
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <param name="recordId">the delegation signer record id</param>
        /// <see>https://developer.dnsimple.com/v2/domains/dnssec/#deleteDomainDelegationSignerRecord</see>
        public EmptyResponse DeleteDelegationSignerRecord(
            long accountId, string domainIdentifier, int recordId)
        {
            var builder = BuildRequestForPath(DsRecordPath(accountId,
                domainIdentifier, recordId));
            builder.Method(Method.DELETE);

            return new EmptyResponse(Execute(builder.Request));
        }
    }

    /// <summary>
    /// Represents a delegation signer record.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct DelegationSignerRecord
    {
        public long Id { get; set; }
        public long DomainId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Algorithm { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Digest { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string DigestType { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Keytag { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}