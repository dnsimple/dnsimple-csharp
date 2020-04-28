using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <inheritdoc cref="DomainsService"/>
    /// <see>https://developer.dnsimple.com/v2/domains/pushes/</see>
    public partial class DomainsService
    {
        /// <summary>
        /// Initiate a push.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <param name="email">The email address of the target DNSimple account.</param>
        /// <returns>The newly created push.</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/pushes/#initiateDomainPush</see>
        public SimpleDnsimpleResponse<PushData> InitiatePush(long accountId,
            string domainIdentifier, string email)
        {
            var request =
                Client.Http.RequestBuilder(InitiatePushPath(accountId,
                    domainIdentifier));
            request.Method(Method.POST);

            request.AddJsonPayload(PushPayload("new_account_email", email));

            return new SimpleDnsimpleResponse<PushData>(Client.Http.Execute(request.Request));
        }

        /// <summary>
        /// List pending pushes for the target account.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <returns>A list of the pending pushes.</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/pushes/#listPushes</see>
        public PaginatedDnsimpleResponse<PushData> ListPushes(long accountId)
        {
            return new PaginatedDnsimpleResponse<PushData>(
                Client.Http.Execute(Client.Http
                    .RequestBuilder(PushPath(accountId))
                    .Request));
        }

        /// <summary>
        /// Accept a push for the target account
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="pushId">The push id</param>
        /// <param name="contactId">A contact that belongs to the target
        /// DNSimple account. The contact will be used as new registrant for
        /// the domain, if the domain is registered with DNSimple.</param>
        /// <see>https://developer.dnsimple.com/v2/domains/pushes/#acceptPush</see>
        public void AcceptPush(long accountId, long pushId, long contactId)
        {
            var request =
                Client.Http.RequestBuilder(PushPath(accountId, pushId));
            request.Method(Method.POST);
            request.AddJsonPayload(PushPayload("contact_id", contactId.ToString()));

            Client.Http.Execute(request.Request);
        }

        /// <summary>
        /// Reject a push for the target account.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="pushId">The push id</param>
        /// <see>https://developer.dnsimple.com/v2/domains/pushes/#rejectPush</see>
        public void RejectPush(int accountId, int pushId)
        {
            var request =
                Client.Http.RequestBuilder(PushPath(accountId, pushId));
            request.Method(Method.DELETE);

            Client.Http.Execute(request.Request);
        }

        private static JsonObject PushPayload(string key, string value)
        {
            var payload = new JsonObject
            {
                new KeyValuePair<string, object>(key, value)
            };
            return payload;
        }
    }

    /// <summary>
    /// Represents a pending push.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct PushData
    {
        public long Id { get; set; }
        public long DomainId { get; set; }
        public long? ContactId { get; set; }
        public long AccountId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}