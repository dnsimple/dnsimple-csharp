using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using static dnsimple.JsonTools<dnsimple.Services.Account>;

namespace dnsimple.Services
{
    /// <summary>
    /// Lists the accounts the authenticated entity has access to.
    /// </summary>
    /// <see>http://developer.dnsimple.com/v2/accounts</see>
    /// <example>
    /// <code>
    ///     var accounts = client.Accounts.List();
    /// </code>
    /// </example>
    public class AccountsService
    {
        private IClient Client { get; }

        /// <summary>
        /// Creates a new instance of the <c>AccountsService</c> by passing
        /// an instance of the DNSimple <c>IClient</c>.
        /// </summary>
        /// <param name="client">An instance of the <c>IClient</c></param>
        /// <see cref="IClient"/>
        public AccountsService(IClient client) => 
            Client = client;

        /// <summary>
        /// Returns a <c>AccountsResponse</c> containing a list of the accounts
        /// the authenticated entity has access to.
        /// </summary>
        /// <returns>An <c>AccountsResponse</c> object</returns>
        /// <see cref="AccountsResponse"/>
        public AccountsResponse List()
        {
            return new AccountsResponse(
                Client.Http.Execute(Client.Http.RequestBuilder("/accounts")
                    .Request));
        }
    }

    /// <summary>
    /// Represents the response from the API call as an object.
    /// </summary>
    /// <see>http://developer.dnsimple.com/v2/accounts</see>
    public class AccountsResponse : ListDnsimpleResponse<AccountsData>
    {
        /// <summary>
        /// Creates a new instance of the <c>AccountsResponse</c> by passing a
        /// <c>JToken</c> representation of the response from the API call.
        /// </summary>
        /// <param name="json"><c>JToken</c> containing the data</param>
        /// <see cref="JToken"/>
        public AccountsResponse(JToken json) => Data = new AccountsData(json);
    }

    /// <summary>
    /// Represents the data returned from the API call by transforming the
    /// incoming JSON into a <c>List</c> of <c>Account</c> objects.
    /// </summary>
    /// <see>http://developer.dnsimple.com/v2/accounts</see>
    public readonly struct AccountsData
    {
        /// <summary>
        /// The <c>List</c> of <c>Account</c>s.
        /// </summary>
        public List<Account> Accounts { get; }

        /// <summary>
        /// Creates a new instance of the <c>AccountData</c> object by passing
        /// a <c>JToken</c> representing the JSON returned by the API call.
        /// </summary>
        /// <param name="json"><c>JToken</c> representing the JSON returned</param>
        /// <see cref="JToken"/>
        public AccountsData(JToken json) => 
            Accounts = DeserializeList(json);
    }
}