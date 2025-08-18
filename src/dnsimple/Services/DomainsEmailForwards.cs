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
        /// Lists email forwards for the domain.
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
        /// Creates a new email forward for the domain.
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

            if (record.To.Trim().Equals("") || record.From.Trim().Equals(""))
                throw new ArgumentException("AliasName or DestinationEmail cannot be blank");

            return new SimpleResponse<EmailForward>(Execute(builder.Request));
        }

        /// <summary>
        /// Retrieves the details of an existing email forward.
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
        /// Permanently deletes an email forward.
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

        public string AliasName { get; set; }

        [Obsolete("Deprecated, use AliasEmail or AliasName instead")]
        public string From
        {
            get
            {
                // If To is accessed, return the value from AliasName
                return AliasName;
            }
            [Obsolete("From is deprecated. Please use AliasName instead.")]
            set
            {
                // If To is set, set the value to AliasName
                AliasName = value;
            }
        }
        public string AliasEmail { get; set; }

        public string DestinationEmail { get; set; }

        [Obsolete("Deprecated, use DestinationEmail instead")]
        public string To
        {
            get
            {
                // If From is accessed, return the value from DestinationEmail
                return DestinationEmail;
            }
            [Obsolete("To is deprecated. Please use DestinationEmail instead.")]
            set
            {
                // If From is set, set the value to DestinationEmail
                DestinationEmail = value;
            }
        }

        public bool Active { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
