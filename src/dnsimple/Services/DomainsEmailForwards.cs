using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.JsonTools<dnsimple.Services.EmailForward>;

namespace dnsimple.Services
{
    /// <inheritdoc cref="DomainsService"/>
    /// <see>https://developer.dnsimple.com/v2/domains/email-forwards/</see>
    /// <see>https://developer.dnsimple.com/v2/openapi.yml</see>
    public partial class DomainsService
    {
        /// <summary>
        /// List email forwards for the domain in the account.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <returns>A list of all email forwards for the domain</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/email-forwards/#listEmailForwards</see>
        public EmailForwardsResponse ListEmailForwards(long accountId,
            string domainIdentifier)
        {
            return new EmailForwardsResponse(Client.Http.Execute(Client.Http
                .RequestBuilder(EmailForwardsPath(accountId, domainIdentifier))
                .Request));
        }

        /// <summary>
        /// List email forwards for the domain in the account.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <param name="options">Options passed to the list (sorting, pagination)</param>
        /// <returns>A list of all email forwards for the domain</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/email-forwards/#listEmailForwards</see>
        public EmailForwardsResponse ListEmailForwards(long accountId,
            string domainIdentifier, DomainEmailForwardsListOptions options)
        {
            var requestBuilder = Client.Http
                .RequestBuilder(EmailForwardsPath(accountId, domainIdentifier));
            requestBuilder.AddParameter(options.UnpackSorting());
            
            if (!options.Pagination.IsDefault())
            {
                requestBuilder.AddParameters(options.UnpackPagination());
            }
            
            return new EmailForwardsResponse(Client.Http.Execute(requestBuilder
                .Request));
        }

        /// <summary>
        /// Creates a email forward for the domain
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <param name="record">The email forward to be added</param>
        /// <returns>The newly created email forward for the domain</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/email-forwards/#createEmailForward</see>
        public EmailForwardResponse CreateEmailForward(long accountId,
            string domainIdentifier, EmailForward record)
        {
            var request =
                Client.Http.RequestBuilder(
                    EmailForwardsPath(accountId, domainIdentifier));
            request.Method(Method.POST);
            request.AddJsonPayload(record);

            return new EmailForwardResponse(
                Client.Http.Execute(request.Request));
        }

        /// <summary>
        /// Retrieves an email forward.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <param name="emailForwardId">The email forward id</param>
        /// <returns>The email forward</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/email-forwards/#getEmailForward</see>
        public EmailForwardResponse GetEmailForward(long accountId,
            string domainIdentifier, int emailForwardId)
        {
            return new EmailForwardResponse(Client.Http.Execute(Client.Http
                .RequestBuilder(EmailForwardPath(accountId, domainIdentifier,
                    emailForwardId)).Request));
        }

        /// <summary>
        /// Deletes an email forward.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <param name="emailForwardId">The email forward id</param>
        /// <see>https://developer.dnsimple.com/v2/domains/email-forwards/#deleteEmailForward</see>
        public void DeleteEmailForward(long accountId, string domainIdentifier,
            int emailForwardId)
        {
            var request = Client.Http.RequestBuilder(
                EmailForwardPath(accountId, domainIdentifier, emailForwardId));
            request.Method(Method.DELETE);

            Client.Http.Execute(request.Request);
        }

        private static string EmailForwardPath(long accountId, string domainIdentifier,
            int emailForwardId)
        {
            return
                $"{EmailForwardsPath(accountId, domainIdentifier)}/{emailForwardId}";
        }

        private static string EmailForwardsPath(long accountId,
            string domainIdentifier)
        {
            return $"{DomainPath(accountId, domainIdentifier)}/email_forwards";
        }
    }

    /// <summary>
    /// Represents the response from the API call containing one
    /// <c>EmailForward</c>.
    /// </summary>
    public class EmailForwardResponse : SimpleDnsimpleResponse<EmailForward>
    {
        public EmailForwardResponse(JToken json) : base(json)
        {
        }
    }

    /// <summary>
    /// Represents the response from the API call containing (potentially)
    /// multiple <c>EmailForward</c> objects inside a <c>EmailForwardsData</c>
    /// object.
    /// </summary>
    public class EmailForwardsResponse : PaginatedDnsimpleResponse<EmailForwardsData>
    {
        public EmailForwardsResponse(JToken response) : base(response) =>
            Data = new EmailForwardsData(response);
    }

    /// <summary>
    /// Represents the <c>struct</c> containing a <c>List</c> of <c>EmailForward</c>
    /// objects.
    /// </summary>
    /// <see cref="List{T}"/>
    /// <see cref="EmailForward"/>
    public readonly struct EmailForwardsData
    {
        public List<EmailForward> EmailForwards { get; }

        public EmailForwardsData(JToken json) =>
            EmailForwards = DeserializeList(json);
    }

    /// <summary>
    /// Represents an email forward.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct EmailForward
    {
        public long Id { get; set; }
        public long DomainId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
    /// <summary>
    /// Defines the options you may want to send to list domain email forwards,
    /// such as pagination and sorting.
    /// </summary>
    /// <see cref="ListOptions"/>
    public class DomainEmailForwardsListOptions : ListOptions
    {
        private const string IdSort = "id";
        private const string FromSort = "from";
        private const string ToSort = "to";

        /// <summary>
        /// Creates a new instance of <c>DomainEmailForwardsListOptions</c>
        /// </summary>
        public DomainEmailForwardsListOptions() =>
            Pagination = new Pagination();
        
        /// <summary>
        /// Sets the order by which to sort by id.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>DomainEmailForwardsListOptions</c></returns>
        public DomainEmailForwardsListOptions SortById(Order order)
        {
            AddSortCriteria(new Sort { Field = IdSort, Order = order});
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by from.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>DomainEmailForwardsListOptions</c></returns>
        public DomainEmailForwardsListOptions SortByFrom(Order order)
        {
            AddSortCriteria(new Sort{Field = FromSort, Order = order});
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by to.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>DomainEmailForwardsListOptions</c></returns>
        public DomainEmailForwardsListOptions SortByTo(Order order)
        {
            AddSortCriteria(new Sort{Field = ToSort, Order = order});
            return this;
        }
    }
}