using System;
using System.Collections.Generic;
using dnsimple.Services.ListOptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <inheritdoc cref="ServiceBase"/>
    /// <see>https://developer.dnsimple.com/v2/certificates/</see>
    public class BillingService : ServiceBase
    {
        /// <inheritdoc cref="ServiceBase"/>
        public BillingService(IClient client) : base(client)
        {
        }

        /// <summary>
        /// Lists the billing charges for an account.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="options">Options passed to the list (filtering, pagination, and sorting)</param>
        /// <returns>A <c>ChargesResponse</c> containing a list of charges for the
        /// account.</returns>
        public PaginatedResponse<Charge> ListCharges(long accountId, ListChargesOptions options = null)
        {
            var builder = BuildRequestForPath(ChargesPath(accountId));
            AddListOptionsToRequest(options, ref builder);

            return new PaginatedResponse<Charge>(Execute(builder.Request));
        }
    }

    /// <summary>
    /// Represents the charge data.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct Charge
    {
        public DateTime InvoicedAt { get; set; }
        public float TotalAmount { get; set; }
        public float BalanceAmount { get; set; }
        public string Reference { get; set; }
        public string State { get; set; }
        public List<ChargeItem> Items { get; set; }
    }

    /// <summary>
    /// Represents the charge item data.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]

    public struct ChargeItem
    {
        public string Description { get; set; }
        public float Amount { get; set; }
        public long ProductId { get; set; }
        public string ProductType { get; set; }
        public string ProductReference { get; set; }
    }

}
