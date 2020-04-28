using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using dnsimple.Services.ListOptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <inheritdoc cref="Service"/>
    /// <see>https://developer.dnsimple.com/v2/domains/</see>
    public partial class DomainsService : Service
    {
        /// <inheritdoc cref="Service"/>
        public DomainsService(IClient client) : base(client)
        {
        }

        /// <summary>
        /// Retrieves the details of an existing domain.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <returns>The domain requested.</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/#getDomain</see>
        public SimpleDnsimpleResponse<Domain> GetDomain(long accountId,
            string domainIdentifier)
        {
            return new SimpleDnsimpleResponse<Domain>(Client.Http.Execute(Client
                .Http
                .RequestBuilder(DomainPath(accountId, domainIdentifier))
                .Request));
        }

        /// <summary>
        /// Lists the domains in the account.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <returns>A <c>DomainResponse</c> containing a list of domains.</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/#listDomains</see>
        public PaginatedDnsimpleResponse<Domain> ListDomains(long accountId)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(DomainsPath(accountId));

            return new PaginatedDnsimpleResponse<Domain>(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Lists the domains in the account.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="options">Options passed to the list (sorting, filtering, pagination)</param>
        /// <returns>A <c>DomainResponse</c> containing a list of domains.</returns>
        /// <see cref="DomainListOptions"/>
        /// <see>https://developer.dnsimple.com/v2/domains/#listDomains</see>
        public PaginatedDnsimpleResponse<Domain> ListDomains(long accountId,
            ListOptionsWithFiltering options)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(DomainsPath(accountId));
            requestBuilder.AddParameter(options.UnpackSorting());
            requestBuilder.AddParameters(options.UnpackFilters());

            if (!options.Pagination.IsDefault())
            {
                requestBuilder.AddParameters(options.UnpackPagination());
            }


            return new PaginatedDnsimpleResponse<Domain>(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Adds a domain to the account.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainName">The name of the domain to be created</param>
        /// <returns>A <c>DomainResponse</c> containing the data of the newly
        /// created domain.</returns>
        /// <see>https://developer.dnsimple.com/v2/domains/#createDomain</see>
        public SimpleDnsimpleResponse<Domain> CreateDomain(long accountId,
            string domainName)
        {
            var request =
                Client.Http.RequestBuilder(DomainsPath(accountId));
            request.Method(Method.POST);

            var parameters = new Collection<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("name", domainName)
            };
            request.AddParameters(parameters);

            return new SimpleDnsimpleResponse<Domain>(
                Client.Http.Execute(request.Request));
        }

        /// <summary>
        /// Permanently deletes a domain from the account.
        /// </summary>
        /// <remarks>It cannot be undone.</remarks>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or id</param>
        /// <see>https://developer.dnsimple.com/v2/domains/#deleteDomain</see>
        public void DeleteDomain(long accountId, string domainIdentifier)
        {
            var request =
                Client.Http.RequestBuilder(DeleteDomainPath(accountId,
                    domainIdentifier));
            request.Method(Method.DELETE);

            Client.Http.Execute(request.Request);
        }
    }

    /// <summary>
    /// Represents a <c>Domain</c>
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/domains/</see>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct Domain
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public long? RegistrantId { get; set; }
        public string Name { get; set; }
        public string UnicodeName { get; set; }
        public string State { get; set; }
        public bool? AutoRenew { get; set; }
        public bool? PrivateWhois { get; set; }
        public string ExpiresOn { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}