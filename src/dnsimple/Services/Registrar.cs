using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <summary>
    /// Provides access to the DNSimple Registrar API.
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/registrar/</see>
    public partial class RegistrarService : ServiceBase
    {
        /// <inheritdoc cref="ServiceBase"/>
        public RegistrarService(IClient client) : base(client)
        {
        }

        /// <summary>
        /// Checks a domain name for availability.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainName">The domain name to check</param>
        /// <returns>The check domain response</returns>
        public SimpleResponse<DomainCheck> CheckDomain(long accountId, string domainName)
        {
            var builder = BuildRequestForPath(DomainCheckPath(accountId, domainName));

            return new SimpleResponse<DomainCheck>(Execute(builder.Request));
        }

        /// <summary>
        /// Retrieves the premium price for a domain.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainName">The domain name</param>
        /// <param name="action">Optional action between "registration",
        /// "renewal", and "transfer". If omitted, it defaults to
        /// "registration".</param>
        /// <returns>The domain premium price response</returns>
        /// <see>https://developer.dnsimple.com/v2/registrar/#getDomainPremiumPrice</see>
        public SimpleResponse<DomainPremiumPrice> GetDomainPremiumPrice(long accountId, string domainName, PremiumPriceCheckAction action)
        {
            var builder = BuildRequestForPath(DomainPremiumPricePath(accountId, domainName));
            builder.AddParameter(new KeyValuePair<string, string>("action", action.ToString().ToLower()));

            return new SimpleResponse<DomainPremiumPrice>(Execute(builder.Request));
        }

        /// <summary>
        /// Get prices for a domain.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainName">The domain name to find the prices</param>
        /// <returns>The domain prices response</returns>
        /// <see>https://developer.dnsimple.com/v2/registrar/#getDomainPrices</see>
        public SimpleResponse<DomainPrices> GetDomainPrices(long accountId, string domainName)
        {
            var builder = BuildRequestForPath(DomainPricesPath(accountId, domainName));

            return new SimpleResponse<DomainPrices>(Execute(builder.Request));
        }

        /// <summary>
        /// Registers a domain name.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainName">The domain name</param>
        /// <param name="domain">The domain to register</param>
        /// <returns>The newly created domain</returns>
        /// <see cref="DomainRegistrationInput"/>
        /// <see>https://developer.dnsimple.com/v2/registrar/#registerDomain</see>
        public SimpleResponse<DomainRegistration> RegisterDomain(long accountId, string domainName, DomainRegistrationInput domain)
        {
            var builder = BuildRequestForPath(DomainRegistrationPath(accountId, domainName));
            builder.Method(Method.POST);
            builder.AddJsonPayload(domain);

            return new SimpleResponse<DomainRegistration>(Execute(builder.Request));
        }

        /// <summary>
        /// Transfer a domain name from another registrar.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainName">The domain name</param>
        /// <param name="transferInput">The transfer command</param>
        /// <returns>The transferred domain</returns>
        /// <see cref="DomainTransfer"/>
        /// <see>https://developer.dnsimple.com/v2/registrar/#transferDomain</see>
        public SimpleResponse<DomainTransfer> TransferDomain(long accountId, string domainName, DomainTransferInput transferInput)
        {
            if (transferInput.AuthCode == null) {
                throw new DnsimpleException("Please provide an AuthCode");
            }
            var builder = BuildRequestForPath(TransferDomainPath(accountId, domainName));
            builder.Method(Method.POST);
            builder.AddJsonPayload(transferInput);

            return new SimpleResponse<DomainTransfer>(Execute(builder.Request));
        }

        /// <summary>
        /// Retrieves the details of an existing domain transfer.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainName">The domain name</param>
        /// <param name="domainTransferId">The domain transfer Id</param>
        /// <returns>The domain transfer</returns>
        /// <see cref="DomainTransfer"/>
        /// <see>https://developer.dnsimple.com/v2/registrar/#getDomainTransfer</see>
        public SimpleResponse<DomainTransfer> GetDomainTransfer(long accountId, string domainName, long domainTransferId)
        {
            var builder = BuildRequestForPath(DomainTransferPath(accountId, domainName, domainTransferId));

            return new SimpleResponse<DomainTransfer>(Execute(builder.Request));
        }

        /// <summary>
        /// Cancels an in progress domain transfer.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainName">The domain name</param>
        /// <param name="domainTransferId">The domain transfer Id</param>
        /// <returns>The domain transfer</returns>
        /// <see cref="DomainTransfer"/>
        /// <see>https://developer.dnsimple.com/v2/registrar/#cancelDomainTransfer</see>
        public SimpleResponse<DomainTransfer> CancelDomainTransfer(long accountId, string domainName, long domainTransferId)
        {
            var builder = BuildRequestForPath(DomainTransferPath(accountId, domainName, domainTransferId));
            builder.Method(Method.DELETE);

            return new SimpleResponse<DomainTransfer>(Execute(builder.Request));
        }

        /// <summary>
        /// Explicitly renews a domain, if the registry supports this function.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainName">The domain name</param>
        /// <param name="input">The domain renewal request</param>
        /// <returns>The renewed domain</returns>
        /// <see>https://developer.dnsimple.com/v2/registrar/#renewDomain</see>
        public SimpleResponse<DomainRenewal> RenewDomain(long accountId, string domainName, DomainRenewalInput input)
        {
            var builder = BuildRequestForPath(DomainRenewalPath(accountId, domainName));
            builder.Method(Method.POST);
            builder.AddJsonPayload(input);

            return new SimpleResponse<DomainRenewal>(Execute(builder.Request));
        }

        /// <summary>
        /// Prepares a domain for transferring out.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainName">The domain name</param>
        /// <see>https://developer.dnsimple.com/v2/registrar/#authorizeDomainTransferOut</see>
        public EmptyResponse TransferDomainOut(long accountId, string domainName)
        {
            var builder = BuildRequestForPath(DomainTransferOutPath(accountId, domainName));
            builder.Method(Method.POST);

            return new EmptyResponse(Execute(builder.Request));
        }
    }

    /// <summary>
    /// Represents the actions you can perform when checking the premium
    /// price domain.
    /// </summary>
    public enum PremiumPriceCheckAction
    {
        Registration,
        Renewal,
        Transfer
    }

    /// <summary>
    /// Represents the domain check.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct DomainCheck
    {
        public string Domain { get; set; }
        public bool Available { get; set; }
        public bool Premium { get; set; }
    }

    /// <summary>
    /// Represents a domain registration.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct DomainRegistration
    {
        public long Id { get; set; }
        public long DomainId { get; set; }
        public long RegistrantId { get; set; }
        public long Period { get; set; }
        public string State { get; set; }
        public bool AutoRenew { get; set; }
        public bool WhoisPrivacy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Represents a domain renewal.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct DomainRenewal
    {
        public long Id { get; set; }
        public long DomainId { get; set; }
        public long Period { get; set; }
        public string State { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Represents a domain transfer.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy),
        ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct DomainTransfer
    {
        public long Id { get; set; }
        public long DomainId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public long RegistrantId { get; set; }

        public string State { get; set; }
        public bool AutoRenew { get; set; }
        public bool WhoisPrivacy { get; set; }
        public string StatusDescription { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Represents the data sent to the API to check the premium price of
    /// a domain.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct DomainPremiumPrice
    {
        public string PremiumPrice { get; set; }
        public string Action { get; set; }
    }

    /// <summary>
    /// Represents a domain prices
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct DomainPrices
    {
        public string Domain { get; set; }
        public bool Premium { get; set; }
        public float RegistrationPrice { get; set; }
        public float RenewalPrice { get; set; }
        public float TransferPrice { get; set; }
    }


    /// <summary>
    /// Represents the data sent to register a domain.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy),
        ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct DomainRegistrationInput
    {
        [JsonProperty(Required = Required.Always)]
        public long RegistrantId { get; set; }

        public bool WhoisPrivacy { get; set; }
        public bool AutoRenew { get; set; }
        public string PremiumPrice { get; set; }
        public List<TldExtendedAttribute> ExtendedAttributes { get; set; }
    }

    /// <summary>
    /// Represents the data sent to renew a domain.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct DomainRenewalInput
    {
        public long Period { get; set; }
        public string PremiumPrice { get; set; }
    }

    /// <summary>
    /// Represents the data sent to transfer a domain.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct DomainTransferInput
    {
        [JsonProperty(Required = Required.Always)]
        public long RegistrantId { get; set; }

        public bool AutoRenew { get; set; }
        public bool WhoisPrivacy { get; set; }

        public string AuthCode { get; set; }
        public string PremiumPrice { get; set; }
        public List<TldExtendedAttribute> ExtendedAttributes { get; set; }
    }

}
