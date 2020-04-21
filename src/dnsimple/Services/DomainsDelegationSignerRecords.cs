using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.JsonTools<dnsimple.Services.DelegationSignerRecord>;

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
        public DelegationSignerRecordsResponse ListDelegationSignerRecords(
            long accountId, string domainIdentifier)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(DsRecordsPath(accountId,
                    domainIdentifier));
            return new DelegationSignerRecordsResponse(
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
        public DelegationSignerRecordsResponse ListDelegationSignerRecords(
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
            
            return new DelegationSignerRecordsResponse(
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
        public DelegationSignerRecordResponse CreateDelegationSignerRecord(
            long accountId, string domainIdentifier,
            DelegationSignerRecord record)
        {
            var request =
                Client.Http.RequestBuilder(DsRecordsPath(accountId,
                    domainIdentifier));
            request.Method(Method.POST);
            request.AddJsonPayload(record);

            return new DelegationSignerRecordResponse(
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
        public DelegationSignerRecordResponse GetDelegationSignerRecord(
            long accountId, string domainIdentifier, long recordId)
        {
            return new DelegationSignerRecordResponse(
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
        public void DeleteDelegationSignerRecord(long accountId, string domainIdentifier, int recordId)
        {
            var request =
                Client.Http.RequestBuilder(DsRecordPath(accountId,
                    domainIdentifier, recordId));
            request.Method(Method.DELETE);

            Client.Http.Execute(request.Request);
        }

        private static string DsRecordsPath(long accountId,
            string domainIdentifier)
        {
            return $"{DomainPath(accountId, domainIdentifier)}/ds_records";
        }

        private static string DsRecordPath(long accountId,
            string domainIdentifier, long recordId)
        {
            return $"{DsRecordsPath(accountId, domainIdentifier)}/{recordId}";
        }
    }

    /// <summary>
    /// Represents the response from the API call containing one
    /// <c>DelegationSignerRecord</c>.
    /// </summary>
    public class DelegationSignerRecordResponse : 
        SimpleDnsimpleResponse<DelegationSignerRecord>
    {
        public DelegationSignerRecordResponse(JToken json) : base(json)
        {
        }
    }

    /// <summary>
    /// Represents the response from the API call containing (potentially)
    /// multiple <c>DelegationSignerRecord</c> objects inside a
    /// <c>DelegationSignerRecordsData</c> object.
    /// </summary>
    public class DelegationSignerRecordsResponse : PaginatedDnsimpleResponse<
        DelegationSignerRecordsData>
    {
        public DelegationSignerRecordsResponse(JToken response) :
            base(response) =>
            Data = new DelegationSignerRecordsData(response);
    }

    /// <summary>
    /// Represents the <c>struct</c> containing a <c>List</c> of
    /// <c>DelegationSignerRecord</c> objects.
    /// </summary>
    /// <see cref="List{T}"/>
    /// <see cref="DelegationSignerRecord"/>
    public readonly struct DelegationSignerRecordsData
    {
        public List<DelegationSignerRecord> DelegationSignerRecords { get; }

        public DelegationSignerRecordsData(JToken json) =>
            DelegationSignerRecords = DeserializeList(json);
    }

    /// <summary>
    /// Represents a delegation signer record.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct DelegationSignerRecord
    {
        public long Id { get; set; }
        public long DomainId { get; set; }
        public string Algorithm { get; set; }
        public string Digest { get; set; }
        public string DigestType { get; set; }
        public string Keytag { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
    /// <summary>
    /// Defines the options you may want to send to list domain delegation
    /// signer records, such as pagination and sorting.
    /// </summary>
    /// <see cref="ListOptions"/>
    public class ListDomainDelegationSignerRecordsOptions : ListOptions {
        private const string IdSort = "id";
        private const string CreatedAtSort = "created_at";

        /// <summary>
        /// Creates a new instance of <c>ListDomainDelegationSignerRecordsOptions</c>
        /// </summary>
        public ListDomainDelegationSignerRecordsOptions() =>
            Pagination = new Pagination();
        
        /// <summary>
        /// Sets the order by which to sort by id.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>ListDomainDelegationSignerRecordsOptions</c></returns>
        public ListDomainDelegationSignerRecordsOptions SortById(Order order)
        {
            AddSortCriteria(new Sort {Field = IdSort, Order = order });
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by created at.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>ListDomainDelegationSignerRecordsOptions</c></returns>
        public ListDomainDelegationSignerRecordsOptions SortByCreatedAt(
            Order order)
        {
            AddSortCriteria(new Sort{Field = CreatedAtSort, Order = order });
            return this;
        }
    }
}