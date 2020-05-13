using dnsimple.Services.ListOptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <summary>
    /// Handles communication with the webhook related
    /// methods of the DNSimple API.
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/webhooks/</see>
    public class WebhooksService : ServiceBase
    {
        /// <inheritdoc cref="ServiceBase" />
        public WebhooksService(IClient client) : base(client)
        {
        }

        /// <summary>
        /// List webhooks in the account.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="options">Options passed to the list (sorting and
        /// pagination).</param>
        /// <returns>The list of webhooks in the account</returns>
        /// <see>https://developer.dnsimple.com/v2/webhooks/#listWebhooks</see>
        public ListDnsimpleResponse<Webhook> ListWebhooks(long accountId,
            ListWebhooksOptions options = null)
        {
            var builder = BuildRequestForPath(WebhooksPath(accountId));

            AddListOptionsToRequest(options, ref builder);

            return new ListDnsimpleResponse<Webhook>(Execute(builder.Request));
        }

        /// <summary>
        /// Create a webhook in the account.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="webhook">The webhook to create</param>
        /// <returns>The newly created webhook</returns>
        /// <see cref="Webhook"/>
        /// <see>https://developer.dnsimple.com/v2/webhooks/#createWebhook</see>
        public SimpleDnsimpleResponse<Webhook> CreateWebhook(long accountId,
            Webhook webhook)
        {
            var builder = BuildRequestForPath(WebhooksPath(accountId));
            builder.Method(Method.POST);
            builder.AddJsonPayload(webhook);

            return new SimpleDnsimpleResponse<Webhook>(
                Execute(builder.Request));
        }

        /// <summary>
        /// Returns a webhook for the account
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="webhookId">The webhook id</param>
        /// <returns>The webhook requested</returns>
        public SimpleDnsimpleResponse<Webhook> GetWebhook(long accountId,
            long webhookId)
        {
            var builder =
                BuildRequestForPath(WebhookPath(accountId, webhookId));
            return new SimpleDnsimpleResponse<Webhook>(
                Execute(builder.Request));
        }

        /// <summary>
        /// Delete a webhook.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="webhookId">The webhook Id</param>
        /// <returns><c>EmptyDnsimpleRequest</c></returns>
        public EmptyDnsimpleResponse DeleteWebhook(long accountId, long webhookId)
        {
            var builder =
                BuildRequestForPath(WebhookPath(accountId, webhookId));
            builder.Method(Method.DELETE);
            return new EmptyDnsimpleResponse(Execute(builder.Request));
        }
    }

    /// <summary>
    /// Represents a webhook
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy),
        ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct Webhook
    {
        public long Id { get; set; }
     
        [JsonProperty(Required = Required.Always)]
        public string Url { get; set; }
    }
}