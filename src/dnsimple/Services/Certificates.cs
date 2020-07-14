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
    public class CertificatesService : ServiceBase
    {
        /// <inheritdoc cref="ServiceBase"/>
        public CertificatesService(IClient client) : base(client)
        {
        }

        /// <summary>
        /// Lists the certificates for a domain.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <param name="options">Options passed to the list (sorting and
        ///  pagination)</param>
        /// <returns>A <c>CertificatesResponse</c> containing a list of zones for the
        /// account.</returns>
        public PaginatedResponse<Certificate> ListCertificates(long accountId, string domainIdentifier, CertificatesListOptions options = null)
        {
            var builder = BuildRequestForPath(CertificatesPath(accountId, domainIdentifier));
            AddListOptionsToRequest(options, ref builder);

            return new PaginatedResponse<Certificate>(Execute(builder.Request));
        }

        /// <summary>
        /// Retrieves the details of an existing certificate.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <param name="certificateId">The certificate id</param>
        /// <returns>The details of the certificate requested</returns>
        /// <see>https://developer.dnsimple.com/v2/certificates/#getCertificate</see>
        public SimpleResponse<Certificate> GetCertificate(long accountId, string domainIdentifier, long certificateId)
        {
            var builder = BuildRequestForPath(CertificatePath(accountId, domainIdentifier, certificateId));

            return new SimpleResponse<Certificate>(Execute(builder.Request));
        }

        /// <summary>
        /// Gets the PEM-encoded certificate, along with the root certificate and intermediate chain.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <param name="certificateId">The certificate id</param>
        /// <returns>The PEM-encoded certificate, along with the root
        /// certificate and intermediate chain.</returns>
        /// <see>https://developer.dnsimple.com/v2/certificates/#downloadCertificate</see>
        public SimpleResponse<CertificateBundle> DownloadCertificate(long accountId, string domainIdentifier, long certificateId)
        {
            var builder = BuildRequestForPath(PemCertificateDownloadPath(accountId, domainIdentifier, certificateId));

            return new SimpleResponse<CertificateBundle>(Execute(builder.Request));
        }

        /// <summary>
        /// Gets the PEM-encoded certificate private key.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <param name="certificateId">The certificate id</param>
        /// <returns>The private key for the certificate requested</returns>
        /// <see>https://developer.dnsimple.com/v2/certificates/#getCertificatePrivateKey</see>
        public SimpleResponse<CertificateBundle> GetCertificatePrivateKey(long accountId, string domainIdentifier, long certificateId)
        {
            var builder = BuildRequestForPath(CertificatePrivateKeyPath(accountId, domainIdentifier, certificateId)); 

            return new SimpleResponse<CertificateBundle>(Execute(builder.Request));
        }

        /// <summary>
        /// Orders a Let's Encrypt certificate with DNSimple.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <param name="attributes">The order attributes</param>
        /// <returns>The order details</returns>
        /// <see cref="LetsencryptCertificateAttributes"/>
        /// <see>https://developer.dnsimple.com/v2/certificates/#purchaseLetsencryptCertificate</see>
        public SimpleResponse<CertificatePurchase> PurchaseLetsEncryptCertificate(long accountId, string domainIdentifier, LetsencryptCertificateAttributes attributes)
        {
            var builder = BuildRequestForPath(PurchaseLetsEncryptCertificatePath(accountId, domainIdentifier));
            builder.Method(Method.POST);
            builder.AddJsonPayload(attributes);

            return new SimpleResponse<CertificatePurchase>(Execute(builder.Request));
        }

        /// <summary>
        /// Issues a Let's Encrypt certificate ordered with DNSimple.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <param name="certificateId">The certificate id</param>
        /// <returns>The certificate data issued</returns>
        /// <see>https://developer.dnsimple.com/v2/certificates/#issueLetsencryptCertificate</see>
        /// <see>https://dnsimple.com/letsencrypt</see>
        public SimpleResponse<Certificate> IssueLetsEncryptCertificate(long accountId, string domainIdentifier, long certificateId)
        {
            var builder = BuildRequestForPath(IssueLetsEncryptCertificatePath(accountId, domainIdentifier, certificateId));
            builder.Method(Method.POST);

            return new SimpleResponse<Certificate>(Execute(builder.Request));
        }

        /// <summary>
        /// Renews a Let's Encrypt certificate ordered with DNSimple.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <param name="certificateId">The certificate id</param>
        /// <param name="attributes">The order attributes</param>
        /// <returns>The order details</returns>
        /// <see cref="LetsencryptCertificateAttributes"/>
        /// <see>https://developer.dnsimple.com/v2/certificates/#purchaseRenewalLetsencryptCertificate</see>
        public SimpleResponse<CertificateRenewal> PurchaseLetsEncryptCertificateRenewal(long accountId, string domainIdentifier, long certificateId, LetsencryptCertificateAttributes attributes)
        {
            var builder = BuildRequestForPath(LetsEncryptCertificateRenewalPath(accountId, domainIdentifier, certificateId));
            builder.Method(Method.POST);
            builder.AddJsonPayload(attributes);

            return new SimpleResponse<CertificateRenewal>(Execute(builder.Request));
        }

        /// <summary>
        /// Issues a Let's Encrypt certificate renewal ordered with DNSimple.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <param name="certificateId">The certificate id</param>
        /// <param name="certificateRenewalId">The certificate renewal id</param>
        /// <returns>The certificate data issued in the renewal</returns>
        public SimpleResponse<Certificate> IssueLetsEncryptCertificateRenewal(long accountId, string domainIdentifier, long certificateId, long certificateRenewalId)
        {
            var builder = BuildRequestForPath(IssueLetsEncryptCertificateRenewalPath(accountId, domainIdentifier, certificateId, certificateRenewalId));
            builder.Method(Method.POST);

            return new SimpleResponse<Certificate>(Execute(builder.Request));
        }
    }

    /// <summary>
    /// Represents the certificate purchase data.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct CertificatePurchase
    {
        public long Id { get; set; }
        public long CertificateId { get; set; }
        public string State { get; set; }
        public bool? AutoRenew { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ExpiresOn { get; set; }
    }

    /// <summary>
    /// Represents the certificate renewal data.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct CertificateRenewal
    {
        public long Id { get; set; }
        public long OldCertificateId { get; set; }
        public long NewCertificateId { get; set; }
        public string State { get; set; }
        public bool? AutoRenew { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    /// <summary>
    /// Represents the set of attributes to purchase a Let's Encrypt certificate.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct LetsencryptCertificateAttributes
    {
        public long? ContactId { get; set; }
        public bool AutoRenew { get; set; }
        public string Name { get; set; }
        public List<string> AlternateNames { get; set; }
    }

    /// <summary>
    /// Represents a Certificate.
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/certificates/#certificate-attributes</see>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct Certificate
    {
        public long Id { get; set; }
        public long DomainId { get; set; }
        public long ContactId { get; set; }
        public string CommonName { get; set; }
        public long Years { get; set; }
        public string Csr { get; set; }
        public string State { get; set; }
        public bool? AutoRenew { get; set; }
        public List<string> AlternateNames { get; set; }
        public string AuthorityIdentifier { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
    }

    /// <summary>
    /// Represents the possible certificates issued by the server (Pem encoded
    /// certificates and private keys).
    /// </summary>
    /// /// <see>https://developer.dnsimple.com/v2/certificates/#certificate-attributes</see>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct CertificateBundle
    {
        [JsonProperty("server")]
        public string ServerCertificate { get; set; }
        
        [JsonProperty("root")]
        public string RootCertificate { get; set; }
        
        [JsonProperty("chain")]
        public List<string> IntermediateCertificates { get; set; }
        
        public string PrivateKey { get; set; }
    }
}
