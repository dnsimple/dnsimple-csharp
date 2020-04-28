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
    public partial class RegistrarService : Service
    {
        /// <inheritdoc cref="Service"/>
        public RegistrarService(IClient client) : base(client)
        {
        }

        /// <summary>
        /// Checks whether a domain is available for registration.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="domainName">The domain name to check</param>
        /// <returns>The check domain response</returns>
        public SimpleDnsimpleResponse<DomainCheckData> CheckDomain(long accountId,
            string domainName)
        {
            return new SimpleDnsimpleResponse<DomainCheckData>(Client.Http.Execute(
                Client.Http.RequestBuilder(DomainCheckPath(accountId,
                    domainName)).Request));
        }

        /// <summary>
        /// Get the premium price for a domain.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="domainName">The domain name</param>
        /// <param name="action">Optional action between "registration",
        /// "renewal", and "transfer". If omitted, it defaults to
        /// "registration".</param>
        /// <returns>The domain premium price response</returns>
        /// <remarks>Please note that a premium price can be different for
        /// registration, renewal, transfer. By default this endpoint returns
        /// the premium price for registration. If you need to check a
        /// different price, you should specify it with the action
        /// param.</remarks>
        /// <see>https://developer.dnsimple.com/v2/registrar/#getDomainPremiumPrice</see>
        public SimpleDnsimpleResponse<DomainPremiumPriceData> GetDomainPremiumPrice(long accountId,
            string domainName, PremiumPriceCheckAction action)
        {
            var requestBuilder = Client
                .Http.RequestBuilder(
                    DomainPremiumPricePath(accountId, domainName));
            requestBuilder.AddParameter(
                new KeyValuePair<string, string>("action",
                    action.ToString().ToLower()));

            return new SimpleDnsimpleResponse<DomainPremiumPriceData>(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Register a domain name with DNSimple.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="domainName">The domain name</param>
        /// <param name="domain">The domain to register</param>
        /// <returns>The newly created domain</returns>
        /// <remarks>Your account must be active for this command to complete
        /// successfully. You will be automatically charged the registration
        /// fee upon successful registration, so please be careful with this
        /// command.</remarks>
        /// <see cref="DomainRegistration"/>
        /// <see>https://developer.dnsimple.com/v2/registrar/#registerDomain</see>
        public SimpleDnsimpleResponse<RegisteredDomain> RegisterDomain(long accountId,
            string domainName, DomainRegistration domain)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(
                    DomainRegistrationPath(accountId, domainName));
            requestBuilder.Method(Method.POST);
            requestBuilder.AddJsonPayload(domain);

            return new SimpleDnsimpleResponse<RegisteredDomain>(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Transfer a domain name from another domain registrar into DNSimple.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="domainName">The domain name</param>
        /// <param name="transfer">The transfer command</param>
        /// <returns>The transferred domain</returns>
        /// <remarks>Your account must be active for this command to complete
        /// successfully. You will be automatically charged the 1-year transfer
        /// fee upon successful transfer, so please be careful with this
        /// command. The transfer may take anywhere from a few minutes up to
        /// 7 days.</remarks>
        /// <see cref="DomainTransfer"/>
        /// <see>https://developer.dnsimple.com/v2/registrar/#transferDomain</see>
        public SimpleDnsimpleResponse<RegisteredDomain> TransferDomain(long accountId,
            string domainName, DomainTransfer transfer)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(
                    DomainTransferPath(accountId, domainName));
            requestBuilder.Method(Method.POST);
            requestBuilder.AddJsonPayload(transfer);

            return new SimpleDnsimpleResponse<RegisteredDomain>(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Renew a domain name already registered with DNSimple.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="domainName">The domain name</param>
        /// <param name="renewal">The domain renewal request</param>
        /// <returns>The renewed domain</returns>
        /// <remarks>Your account must be active for this command to complete
        /// successfully. You will be automatically charged the renewal fee
        /// upon successful renewal, so please be careful with this
        /// command.</remarks>
        /// <see>https://developer.dnsimple.com/v2/registrar/#renewDomain</see>
        public SimpleDnsimpleResponse<RegisteredDomain> RenewDomain(long accountId,
            string domainName, DomainRenewal renewal)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(
                    DomainRenewalPath(accountId, domainName));
            requestBuilder.Method(Method.POST);
            requestBuilder.AddJsonPayload(renewal);

            return new SimpleDnsimpleResponse<RegisteredDomain>(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Prepare a domain for transferring out.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="domainName">The domain name</param>
        /// <remarks>This will unlock a domain and send the authorization code
        /// to the domain’s administrative contact.</remarks>
        /// <see>https://developer.dnsimple.com/v2/registrar/#authorizeDomainTransferOut</see>
        public void TransferDomainOut(long accountId, string domainName)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(
                    DomainTransferOutPath(accountId, domainName));
            requestBuilder.Method(Method.POST);

            Client.Http.Execute(requestBuilder.Request);
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
    /// Represents a registered domain.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct RegisteredDomain
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
    /// Represents the domain check.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct DomainCheckData
    {
        public string Domain { get; set; }
        public bool Available { get; set; }
        public bool Premium { get; set; }
    }

    /// <summary>
    /// Represents the data sent to the API to check the premium price of
    /// a domain.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct DomainPremiumPriceData
    {
        public string PremiumPrice { get; set; }
        public string Action { get; set; }
    }

    // TODO : Add the extended attributes
    /// <summary>
    /// Represents a domain registration.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct DomainRegistration
    {
        [JsonProperty(Required = Required.Always)]
        public long RegistrantId { get; set; }

        public bool WhoisPrivacy { get; set; }
        public bool AutoRenew { get; set; }
        public string PremiumPrice { get; set; }
    }

    /// <summary>
    /// Represents the data sent to the API when transferring a domain.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct DomainTransfer
    {
        [JsonProperty(Required = Required.Always)]
        public long RegistrantId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string AuthCode { get; set; }

        public bool WhoisPrivacy { get; set; }
        public bool AutoRenew { get; set; }
        public string PremiumPrice { get; set; }
    }

    /// <summary>
    /// Represents the data sent to renew a domain.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct DomainRenewal
    {
        public long Period { get; set; }
        public string PremiumPrice { get; set; }
    }
}