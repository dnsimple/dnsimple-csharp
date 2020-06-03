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
        /// List the certificates for a domain in the account.
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
        /// Get the details of a certificate.
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
        /// Get the PEM-encoded certificate, along with the root certificate
        /// and intermediate chain.
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
        /// Get the PEM-encoded certificate private key.
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
        /// Purchase a Let’s Encrypt certificate with DNSimple.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default certificate name is www and covers both the root
        /// domain (e.g. example.com) and the www subdomain (e.g. www.example.com).
        /// You can choose a custom name (like api), which is valid only for
        /// https://api.example.com. Custom names require a subscription to a
        /// Professional or Business plan.
        /// </para>
        /// <para>
        /// A certificate can be purchased for multiple subdomains. We call
        /// them alternate names or Subject Alternative Name (SAN).
        /// By default, a certificate doesn't have alternate names.
        /// You can purchase a single certificate for both https://docs.example.com
        /// and https://status.example.com, alongside https://example.com.
        ///</para>
        /// <para>
        /// Alternate names require a subscription to a Professional or
        /// Business plan.
        /// </para>
        /// <para>
        /// To request a wildcard certificate that’s valid for an unlimited
        /// number of names that belong to a single subdomain level, use *
        /// (e.g. *.example.com).
        /// Let’s Encrypt wildcard certificates is a feature that is only
        /// available to the following new plans: Professional or Business.
        /// If the feature is not enabled, you will receive an HTTP 412
        /// response code.
        /// </para>
        /// <para>
        /// By default, a certificate isn’t auto-renewed when it expires.
        /// Certificates with auto-renewal disabled may be renewed manually.
        /// You may also purchase the certificate once and select the
        /// auto-renewal option. With auto-renewal enabled, our system
        /// automatically renews a certificate before it expires. Notifications
        /// for renewed certificates are sent via email, and a webhook is fired
        /// when a new certificate is available. You’ll still have to install
        /// the renewed certificate.
        /// </para>
        /// </remarks>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <param name="order">The order object</param>
        /// <returns>The certificate ordered</returns>
        /// <see cref="CertificateOrder"/>
        /// <see>https://developer.dnsimple.com/v2/certificates/#purchaseLetsencryptCertificate</see>
        public SimpleResponse<CertificateOrdered> PurchaseLetsEncryptCertificate(long accountId, string domainIdentifier, CertificateOrder order)
        {
            var builder = BuildRequestForPath(PurchaseLetsEncryptCertificatePath(accountId, domainIdentifier));
            builder.Method(Method.POST);
            builder.AddJsonPayload(order);

            return new SimpleResponse<CertificateOrdered>(Execute(builder.Request));
        }

        /// <summary>
        /// Issue a Let’s Encrypt certificate purchased with DNSimple.
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
        /// Renew a Let’s Encrypt certificate purchased with DNSimple.
        /// </summary>
        /// <param name="accountId">The account ID</param>
        /// <param name="domainIdentifier">The domain name or ID</param>
        /// <param name="certificateId">The certificate id</param>
        /// <param name="renewal">The renewal object.</param>
        /// <returns>The renewal data.</returns>
        /// <see>https://developer.dnsimple.com/v2/certificates/#purchaseRenewalLetsencryptCertificate</see>
        public SimpleResponse<LetsEncryptRenewal> PurchaseLetsEncryptCertificateRenewal(long accountId, string domainIdentifier, long certificateId, LetsEncryptRenewal renewal)
        {
            var builder = BuildRequestForPath(LetsEncryptCertificateRenewalPath(accountId, domainIdentifier, certificateId));
            builder.Method(Method.POST);
            builder.AddJsonPayload(renewal);

            return new SimpleResponse<LetsEncryptRenewal>(Execute(builder.Request));
        }

        /// <summary>
        /// Issue a Let’s Encrypt certificate renewed with DNSimple.
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
    /// Represents the letsencrypt renewal data.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy), ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct LetsEncryptRenewal
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
    /// Represents the certificate purchase order data.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct CertificateOrder
    {
        [JsonProperty(Required = Required.Always)]
        public long? ContactId { get; set; }
        public bool AutoRenew { get; set; }
        public string Name { get; set; }
        public List<string> AlternateNames { get; set; }
    }

    /// <summary>
    /// Represents the order of a letsencrypt certificate.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct CertificateOrdered
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
    /// Represents a Certificate
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/certificates/#certificate-attributes</see>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct Certificate
    {
        public long Id { get; set; }
        public long DomainId { get; set; }
        public long ContactId { get; set; }
        public string Name { get; set; }
        public string CommonName { get; set; }
        public long Years { get; set; }
        public string Csr { get; set; }
        public string State { get; set; }
        public bool? AutoRenew { get; set; }
        public List<string> AlternateNames { get; set; }
        public string AuthorityIdentifier { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? ExpiresOn { get; set; }
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
