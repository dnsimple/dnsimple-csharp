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
        /// <param name="accountId">The account id</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <returns>A list of delegation signer records wrapped in a response</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/dnssec/#listDomainDelegationSignerRecords</see>
        public PaginatedDnsimpleResponse<DelegationSignerRecord> ListDelegationSignerRecords(
            long accountId, string domainIdentifier)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(DsRecordsPath(accountId,
                    domainIdentifier));
            return new PaginatedDnsimpleResponse<DelegationSignerRecord>(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// List delegation signer records for the domain in the account.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <param name="options">Options passed to the list (sorting, pagination)</param>
        /// <returns>A list of delegation signer records wrapped in a response</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/dnssec/#listDomainDelegationSignerRecords</see>
        public PaginatedDnsimpleResponse<DelegationSignerRecord> ListDelegationSignerRecords(
            long accountId, string domainIdentifier, ListDomainDelegationSignerRecordsOptions options)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(DsRecordsPath(accountId,
                    domainIdentifier));
            
            requestBuilder.AddParameter(options.UnpackSorting());
            
            if (!options.Pagination.IsDefault())
            {
                requestBuilder.AddParameters(options.UnpackPagination());
            }
            
            return new PaginatedDnsimpleResponse<DelegationSignerRecord>(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// You only need to create a delegation signer record manually if your
        /// domain is registered with DNSimple but hosted with another DNS
        /// provider that is signing your zone. To enable DNSSEC on a domain
        /// that is hosted with DNSimple, use the DNSSEC enable endpoint.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <param name="record">The delegation signer record to be added</param>
        /// <see cref="DelegationSignerRecord"/>
        /// <returns>The newly created delegation signer record.</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/dnssec/#createDomainDelegationSignerRecord</see>
        /// <see>"https://tools.ietf.org/html/rfc4034"</see>
        public SimpleDnsimpleResponse<DelegationSignerRecord> CreateDelegationSignerRecord(
            long accountId, string domainIdentifier,
            DelegationSignerRecord record)
        {
            var request =
                Client.Http.RequestBuilder(DsRecordsPath(accountId,
                    domainIdentifier));
            request.Method(Method.POST);
            request.AddJsonPayload(record);

            return new SimpleDnsimpleResponse<DelegationSignerRecord>(
                Client.Http.Execute(request.Request));
        }

        /// <summary>
        /// Retrieves a delegation signer record.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <param name="recordId">The delegation signer record id</param>
        /// <returns>The delegation signer record</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/dnssec/#getDomainDelegationSignerRecord</see>
        public SimpleDnsimpleResponse<DelegationSignerRecord> GetDelegationSignerRecord(
            long accountId, string domainIdentifier, long recordId)
        {
            return new SimpleDnsimpleResponse<DelegationSignerRecord>(
                Client.Http.Execute(Client.Http
                    .RequestBuilder(DsRecordPath(accountId, domainIdentifier,
                        recordId)).Request));
        }

        /// <summary>
        /// Deletes a delegation signer record
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <param name="recordId">the delegation signer record id</param>
        /// <see>https://developer.dnsimple.com/v2/domains/dnssec/#deleteDomainDelegationSignerRecord</see>
        public EmptyDnsimpleResponse DeleteDelegationSignerRecord(long accountId, string domainIdentifier, int recordId)
        {
            var request =
                Client.Http.RequestBuilder(DsRecordPath(accountId,
                    domainIdentifier, recordId));
            request.Method(Method.DELETE);

            return new EmptyDnsimpleResponse(Client.Http.Execute(request.Request));
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