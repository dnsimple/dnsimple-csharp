using System;
using System.Collections.Generic;
using dnsimple.Services.ListOptions;
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
            var builder = BuildRequestForPath(RegisterDomainPath(accountId, domainName));
            builder.Method(Method.POST);
            builder.AddJsonPayload(domain);

            return new SimpleResponse<DomainRegistration>(Execute(builder.Request));
        }

        /// <summary>
        /// Retrieve the details of an existing domain registration.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainName">The domain name</param>
        /// <param name="domainRegistrationId">The domain registration Id</param>
        /// <returns>The domain registration</returns>
        /// <see cref="DomainRegistration"/>
        /// <see>https://developer.dnsimple.com/v2/registrar/#getDomainRegistration</see>
        public SimpleResponse<DomainRegistration> GetDomainRegistration(long accountId, string domainName, long domainRegistrationId)
        {
            var builder = BuildRequestForPath(DomainRegistrationPath(accountId, domainName, domainRegistrationId));

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
            if (transferInput.AuthCode == null)
            {
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
        /// Checks what are the requirements for a registrant change.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="input">The check registrant change command</param>
        /// <returns>RegistrantChangeCheck</returns>
        public SimpleResponse<RegistrantChangeCheck> CheckRegistrantChange(long accountId, CheckRegistrantChangeInput input)
        {
            var builder = BuildRequestForPath(CheckRegistrantChangePath(accountId));
            builder.Method(Method.POST);
            builder.AddJsonPayload(input);

            return new SimpleResponse<RegistrantChangeCheck>(Execute(builder.Request));
        }

        /// <summary>
        /// Retrieves the details of an existing registrant change.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="registrantChangeId">The registrant change ID</param>
        /// <returns>RegistrantChange</returns>
        public SimpleResponse<RegistrantChange> GetRegistrantChange(long accountId, long registrantChangeId)
        {
            var builder = BuildRequestForPath(RegistrantChangePath(accountId, registrantChangeId));

            return new SimpleResponse<RegistrantChange>(Execute(builder.Request));
        }

        /// <summary>
        /// Start registrant change.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="input" cref="CreateRegistrantChangeInput">the attributes to start a registrant change</param>
        /// <returns>RegistrantChange</returns>
        public SimpleResponse<RegistrantChange> CreateRegistrantChange(long accountId, CreateRegistrantChangeInput input)
        {
            var builder = BuildRequestForPath(RegistrantChangesPath(accountId));
            builder.Method(Method.POST);
            builder.AddJsonPayload(input);

            return new SimpleResponse<RegistrantChange>(Execute(builder.Request));
        }


        /// <summary>
        /// Lists the registrant changes for the account.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="options" cref="RegistrantChangesListOptions">Options passed to the list (sorting and
        /// pagination).</param>
        /// <returns>A list of all the registrant changes for the account <c>RegistrantChange</c></returns>
        public PaginatedResponse<RegistrantChange> ListRegistrantChanges(long accountId, RegistrantChangesListOptions options = null)
        {
            var builder = BuildRequestForPath(RegistrantChangesPath(accountId));
            AddListOptionsToRequest(options, ref builder);

            return new PaginatedResponse<RegistrantChange>(Execute(builder.Request));
        }

        /// <summary>
        /// Cancels a registrant change.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="registrantChangeId">The registrant change ID</param>
        /// <returns>Returns an empty response if cancellation was successful immediately.
        /// If cancellation is not immediate, returns a registrant change.</returns>
        public SimpleResponseOrEmpty<RegistrantChange> DeleteRegistrantChange(long accountId, long registrantChangeId)
        {
            var builder = BuildRequestForPath(RegistrantChangePath(accountId, registrantChangeId));
            builder.Method(Method.DELETE);

            return new SimpleResponseOrEmpty<RegistrantChange>(Execute(builder.Request));
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
            var builder = BuildRequestForPath(RenewDomainPath(accountId, domainName));
            builder.Method(Method.POST);
            builder.AddJsonPayload(input);

            return new SimpleResponse<DomainRenewal>(Execute(builder.Request));
        }

        /// <summary>
        /// Retrieves the details of an existing domain renewal.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainName">The domain name</param>
        /// <param name="domainRenewalId">The domain renewal Id</param>
        /// <returns>The domain renewal</returns>
        /// <see cref="DomainRenewal"/>
        /// <see>https://developer.dnsimple.com/v2/registrar/#getDomainRenewal</see>
        public SimpleResponse<DomainRenewal> GetDomainRenewal(long accountId, string domainName, long domainRenewalId)
        {
            var builder = BuildRequestForPath(DomainRenewalPath(accountId, domainName, domainRenewalId));

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
    /// Represents a domain registrant change check.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct RegistrantChangeCheck
    {
        public long ContactId { get; set; }
        public long DomainId { get; set; }
        public List<TldExtendedAttribute> ExtendedAttributes { get; set; }
        public bool RegistryOwnerChange { get; set; }
    }

    /// <summary>
    /// Represents a registrant change.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct RegistrantChange
    {
        public long Id { get; set; }
        public long AccountId { get; set; }
        public long ContactId { get; set; }
        public long DomainId { get; set; }
        public string State { get; set; }
        public Dictionary<string, string> ExtendedAttributes { get; set; }
        public bool RegistryOwnerChange { get; set; }
        public string IrtLockLiftedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
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

    /// <summary>
    /// Represents the data sent to check requirements for registrant change.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct CheckRegistrantChangeInput
    {
        /// <summary>
        /// The contact ID to be used as the new registrant.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public long ContactId { get; set; }

        /// <summary>
        /// The domain ID for which the registrant change is being requested.
        /// Can be a string or an int.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public object DomainId { get; set; }
    }

    /// <summary>
    /// Represents the data sent to create a registrant change.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct CreateRegistrantChangeInput
    {
        /// <summary>
        /// The contact ID to be used as the new registrant.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public long ContactId { get; set; }

        /// <summary>
        /// The domain ID for which the registrant change is being requested.
        /// Can be a string or an int.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public object DomainId { get; set; }

        /// <summary>
        /// The extended attributes to be used as the new registrant.
        /// </summary>
        public Dictionary<string, string> ExtendedAttributes { get; set; }
    }

}
