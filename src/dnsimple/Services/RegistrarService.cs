using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;

namespace dnsimple.Services
{
    /// <summary>
    /// Provides access to the DNSimple Registrar API.
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/registrar/</see>
    public class RegistrarService : Service
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
        public DomainCheckResponse CheckDomain(long accountId,
            string domainName)
        {
            return new DomainCheckResponse(Client.Http.Execute(
                Client.Http.RequestBuilder(DomainCheckPath(accountId,
                    domainName)).Request));
        }

        public DomainPremiumPriceResponse GetDomainPremiumPrice(long accountId,
            string domainName, PremiumPriceCheckAction action)
        {
            var requestBuilder = Client
                .Http.RequestBuilder(
                    DomainPremiumPricePath(accountId, domainName));
            requestBuilder.AddParameter(
                new KeyValuePair<string, string>("action",
                    action.ToString().ToLower()));

            return new DomainPremiumPriceResponse(
                Client.Http.Execute(requestBuilder.Request));
        }

        public DomainRegistrationResponse RegisterDomain(long accountId,
            string domainName, DomainRegistration domain)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(
                    DomainRegistrationPath(accountId, domainName));
            requestBuilder.Method(Method.POST);
            requestBuilder.AddJsonPayload(domain);

            return new DomainRegistrationResponse(
                Client.Http.Execute(requestBuilder.Request));
        }

        public DomainRegistrationResponse TransferDomain(long accountId,
            string domainName, DomainTransfer transfer)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(
                    DomainTransferPath(accountId, domainName));
            requestBuilder.Method(Method.POST);
            requestBuilder.AddJsonPayload(transfer);

            return new DomainRegistrationResponse(
                Client.Http.Execute(requestBuilder.Request));
        }

        public DomainRegistrationResponse RenewDomain(long accountId,
            string domainName, DomainRenewal renewal)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(
                    DomainRenewalPath(accountId, domainName));
            requestBuilder.Method(Method.POST);
            requestBuilder.AddJsonPayload(renewal);

            return new DomainRegistrationResponse(
                Client.Http.Execute(requestBuilder.Request));
        }

        public void TransferDomainOut(long accountId, string domainName)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(
                    DomainTransferOutPath(accountId, domainName));
            requestBuilder.Method(Method.POST);

            Client.Http.Execute(requestBuilder.Request);
        }

        private string DomainTransferOutPath(long accountId, string domainName)
        {
            return
                $"{RegistrarPath(accountId, domainName)}/authorize_transfer_out";
        }

        private string DomainRenewalPath(long accountId, string domainName)
        {
            return $"{RegistrarPath(accountId, domainName)}/renewals";
        }

        private string DomainTransferPath(long accountId, string domainName)
        {
            return $"{RegistrarPath(accountId, domainName)}/transfers";
        }

        private static string DomainRegistrationPath(long accountId,
            string domainName)
        {
            return $"{RegistrarPath(accountId, domainName)}/registrations";
        }

        private static string DomainCheckPath(long accountId, string domainName)
        {
            return $"{RegistrarPath(accountId, domainName)}/check";
        }

        private static string DomainPremiumPricePath(long accountId,
            string domainName)
        {
            return $"{RegistrarPath(accountId, domainName)}/premium_price";
        }

        private static string RegistrarPath(long accountId, string domainName)
        {
            return $"{accountId}/registrar/domains/{domainName}";
        }
    }

    public class DomainRegistrationResponse : SimpleDnsimpleResponse<RegisteredDomain>
    {
        public DomainRegistrationResponse(JToken json) : base(json)
        {
        }
    }

    public class DomainCheckResponse : SimpleDnsimpleResponse<DomainCheckData>
    {
        public DomainCheckResponse(JToken json) : base(json)
        {
        }
    }

    public class DomainPremiumPriceResponse : SimpleDnsimpleResponse<
            DomainPremiumPriceData>
    {
        public DomainPremiumPriceResponse(JToken json) : base(json)
        {
        }
    }

    public enum PremiumPriceCheckAction
    {
        Registration,
        Renewal,
        Transfer
    }

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

    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct DomainCheckData
    {
        public string Domain { get; set; }
        public bool Available { get; set; }
        public bool Premium { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct DomainPremiumPriceData
    {
        public string PremiumPrice { get; set; }
        public string Action { get; set; }
    }

    // TODO : Add the extended attributes
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct DomainRegistration
    {
        [JsonProperty(Required = Required.Always)]
        public long RegistrantId { get; set; }

        public bool WhoisPrivacy { get; set; }
        public bool AutoRenew { get; set; }
        public string PremiumPrice { get; set; }
    }

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

    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct DomainRenewal
    {
        public long Period { get; set; }
        public string PremiumPrice { get; set; }
    }
}