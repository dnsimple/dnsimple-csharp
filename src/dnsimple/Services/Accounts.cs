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
    /// <see>https://developer.dnsimple.com/v2/accounts/</see>
    public class AccountsService : ServiceBase
    {
        /// <inheritdoc cref="ServiceBase"/>
        public AccountsService(IClient client) : base(client)
        {
        }

        /// <summary>
        /// Lists the accounts the current authenticated entity has access to.
        /// </summary>
        /// <returns>An <c>AccountsResponse</c> containing a list of accounts.</returns>
        /// <see>https://developer.dnsimple.com/v2/accounts/#listAccounts</see>
        public ListDnsimpleResponse<Account> List()
        {
            return new ListDnsimpleResponse<Account>(
                Execute(BuildRequestForPath("/accounts")
                    .Request));
        }
    }
}