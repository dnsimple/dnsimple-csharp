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
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <param name="options">Options passed to the list (sorting, pagination)</param>
        /// <returns>A list of all email forwards for the domain</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/email-forwards/#listEmailForwards</see>
        public PaginatedResponse<EmailForward> ListEmailForwards(long accountId, string domainIdentifier, DomainEmailForwardsListOptions options = null)
        {
            var builder =
                BuildRequestForPath(EmailForwardsPath(accountId,
                    domainIdentifier));

            AddListOptionsToRequest(options, ref builder);

            return new PaginatedResponse<EmailForward>(Execute(builder.Request));
        }

        /// <summary>
        /// Creates a email forward for the domain
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <param name="record">The email forward to be added</param>
        /// <returns>The newly created email forward for the domain</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/email-forwards/#createEmailForward</see>
        public SimpleResponse<EmailForward> CreateEmailForward(long accountId, string domainIdentifier, EmailForward record)
        {
            var builder = BuildRequestForPath(EmailForwardsPath(accountId, domainIdentifier));
            builder.Method(Method.POST);
            builder.AddJsonPayload(record);

            if(record.To.Trim().Equals("") || record.From.Trim().Equals(""))
                throw new ArgumentException("From or To cannot be blank");
            
            return new SimpleResponse<EmailForward>(Execute(builder.Request));
        }

        /// <summary>
        /// Retrieves an email forward.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <param name="emailForwardId">The email forward id</param>
        /// <returns>The email forward</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/email-forwards/#getEmailForward</see>
        public SimpleResponse<EmailForward> GetEmailForward(long accountId, string domainIdentifier, int emailForwardId)
        {
            var builder = BuildRequestForPath(EmailForwardPath(accountId, domainIdentifier, emailForwardId)); 
            
            return new SimpleResponse<EmailForward>(Execute(builder.Request));
        }

        /// <summary>
        /// Deletes an email forward.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <param name="emailForwardId">The email forward id</param>
        /// <see>https://developer.dnsimple.com/v2/domains/email-forwards/#deleteEmailForward</see>
        public EmptyResponse DeleteEmailForward(long accountId, string domainIdentifier, int emailForwardId)
        {
            var builder = BuildRequestForPath(EmailForwardPath(accountId, domainIdentifier, emailForwardId));
            builder.Method(Method.DELETE);

            return new EmptyResponse(Execute(builder.Request));
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
