using System;
using dnsimple.Services.ListOptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.Paths;

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
        public PaginatedDnsimpleResponse<EmailForward> ListEmailForwards(long accountId,
            string domainIdentifier)
        {
            return new PaginatedDnsimpleResponse<EmailForward>(Client.Http.Execute(Client.Http
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
        public PaginatedDnsimpleResponse<EmailForward> ListEmailForwards(long accountId,
            string domainIdentifier, DomainEmailForwardsListOptions options)
        {
            var requestBuilder = Client.Http
                .RequestBuilder(EmailForwardsPath(accountId, domainIdentifier));
            requestBuilder.AddParameter(options.UnpackSorting());
            
            if (!options.Pagination.IsDefault())
            {
                requestBuilder.AddParameters(options.UnpackPagination());
            }
            
            return new PaginatedDnsimpleResponse<EmailForward>(Client.Http.Execute(requestBuilder
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
        public SimpleDnsimpleResponse<EmailForward> CreateEmailForward(long accountId,
            string domainIdentifier, EmailForward record)
        {
            var request =
                Client.Http.RequestBuilder(
                    EmailForwardsPath(accountId, domainIdentifier));
            request.Method(Method.POST);
            request.AddJsonPayload(record);

            return new SimpleDnsimpleResponse<EmailForward>(
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
        public SimpleDnsimpleResponse<EmailForward> GetEmailForward(long accountId,
            string domainIdentifier, int emailForwardId)
        {
            return new SimpleDnsimpleResponse<EmailForward>(Client.Http.Execute(Client.Http
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
        public EmptyDnsimpleResponse DeleteEmailForward(long accountId, string domainIdentifier,
            int emailForwardId)
        {
            var request = Client.Http.RequestBuilder(
                EmailForwardPath(accountId, domainIdentifier, emailForwardId));
            request.Method(Method.DELETE);

            return new EmptyDnsimpleResponse(Client.Http.Execute(request.Request));
        }
    }

    /// <summary>
    /// Represents an email forward.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct EmailForward
    {
        public long Id { get; set; }
        public long DomainId { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string From { get; set; }
        [JsonProperty(Required = Required.Always)]
        public string To { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}