using System;
using System.Collections.Generic;
using dnsimple.Services.ListOptions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using static dnsimple.Services.JsonTools<dnsimple.Services.Certificate>;
using static dnsimple.Services.Paths;

namespace dnsimple.Services
{
    /// <inheritdoc cref="Service"/>
    /// <see>https://developer.dnsimple.com/v2/certificates/</see>
    public class CertificatesService : Service
    {
        /// <inheritdoc cref="Service"/>
        public CertificatesService(IClient client) : base(client)
        {
        }

        /// <summary>
        /// List the certificates for a domain in the account.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="domainName">The domain name or id</param>
        /// <returns>A <c>CertificatesResponse</c> containing a list of zones for the
        /// account.</returns>
        /// <see>https://developer.dnsimple.com/v2/certificates/#listCertificates</see>
        public PaginatedDnsimpleResponse<CertificatesData> ListCertificates(
            long accountId,
            string domainName)
        {
            return new PaginatedDnsimpleResponse<CertificatesData>(
                Client.Http.Execute(
                    Client.Http
                        .RequestBuilder(CertificatesPath(accountId, domainName))
                        .Request));
        }

        /// <summary>
        /// List the certificates for a domain in the account.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="domainName">The domain name or id</param>
        /// <param name="options">Options passed to the list (sorting and
        ///  pagination)</param>
        /// <returns>A <c>CertificatesResponse</c> containing a list of zones for the
        /// account.</returns>
        public PaginatedDnsimpleResponse<CertificatesData> ListCertificates(
            long accountId,
            string domainName, CertificatesListOptions options)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(
                    CertificatesPath(accountId, domainName));
            requestBuilder.AddParameter(options.UnpackSorting());

            if (!options.Pagination.IsDefault())
            {
                requestBuilder.AddParameters(options.UnpackPagination());
            }

            return new PaginatedDnsimpleResponse<CertificatesData>(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Get the details of a certificate.
        /// </summary>
        /// <param name="accountId">The account Id</param>
        /// <param name="domainName">The domain name or id</param>
        /// <param name="certificateId">The certificate id</param>
        /// <returns>The details of the certificate requested</returns>
        /// <see>https://developer.dnsimple.com/v2/certificates/#getCertificate</see>
        public SimpleDnsimpleResponse<Certificate> GetCertificate(
            long accountId,
            string domainName, long certificateId)
        {
            return new SimpleDnsimpleResponse<Certificate>(Client.Http.Execute(
                Client.Http
                    .RequestBuilder(CertificatePath(accountId, domainName,
                        certificateId)).Request));
        }

        /// <summary>
        /// Get the PEM-encoded certificate, along with the root certificate
        /// and intermediate chain.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="domainName">The domain name or id</param>
        /// <param name="certificateId">The certificate id</param>
        /// <returns>The PEM-encoded certificate, along with the root
        /// certificate and intermediate chain.</returns>
        /// <see>https://developer.dnsimple.com/v2/certificates/#downloadCertificate</see>
        public SimpleDnsimpleResponse<PemEncodedCertificate>
            DownloadCertificate(long accountId,
                string domainName, long certificateId)
        {
            return new SimpleDnsimpleResponse<PemEncodedCertificate>(
                Client.Http.Execute(Client.Http
                    .RequestBuilder(PemCertificateDownloadPath(accountId,
                        domainName, certificateId)).Request));
        }

        /// <summary>
        /// Get the PEM-encoded certificate private key.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="domainName">The domain name or id</param>
        /// <param name="certificateId">The certificate id</param>
        /// <returns>The private key for the certificate requested</returns>
        /// <see>https://developer.dnsimple.com/v2/certificates/#getCertificatePrivateKey</see>
        public SimpleDnsimpleResponse<PrivateKeyData> GetCertificatePrivateKey(
            long accountId, string domainName, long certificateId)
        {
            return new SimpleDnsimpleResponse<PrivateKeyData>(
                Client.Http.Execute(Client.Http
                    .RequestBuilder(CertificatePrivateKeyPath(accountId,
                        domainName, certificateId)).Request));
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
        /// By default, a certificate doesn’t have alternate names.
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
        /// <param name="accountId">The account id</param>
        /// <param name="domainName">The domain name or id</param>
        /// <param name="order">The order object</param>
        /// <returns>The certificate ordered</returns>
        /// <see cref="CertificateOrder"/>
        /// <see>https://developer.dnsimple.com/v2/certificates/#purchaseLetsencryptCertificate</see>
        public SimpleDnsimpleResponse<CertificateOrdered>
            PurchaseLetsEncryptCertificate(
                long accountId, string domainName, CertificateOrder order)
        {
            var requestBuilder = Client.Http.RequestBuilder(
                PurchaseLetsEncryptCertificatePath(accountId, domainName));
            requestBuilder.Method(Method.POST);
            requestBuilder.AddJsonPayload(order);

            return new SimpleDnsimpleResponse<CertificateOrdered>(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Issue a Let’s Encrypt certificate purchased with DNSimple.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="domainName">The domain name or id</param>
        /// <param name="certificateId">The certificate id</param>
        /// <returns>The certificate data issued</returns>
        /// <see>https://developer.dnsimple.com/v2/certificates/#issueLetsencryptCertificate</see>
        /// <see>https://dnsimple.com/letsencrypt</see>
        public SimpleDnsimpleResponse<Certificate> IssueLetsEncryptCertificate(
            long accountId,
            string domainName, long certificateId)
        {
            var requestBuilder = Client.Http.RequestBuilder(
                IssueLetsEncryptCertificatePath(accountId, domainName,
                    certificateId));
            requestBuilder.Method(Method.POST);

            return new SimpleDnsimpleResponse<Certificate>(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Renew a Let’s Encrypt certificate purchased with DNSimple.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="domainName">The domain name or id</param>
        /// <param name="certificateId">The certificate id</param>
        /// <param name="renewal">The renewal object.</param>
        /// <returns>The renewal data.</returns>
        /// <see>https://developer.dnsimple.com/v2/certificates/#purchaseRenewalLetsencryptCertificate</see>
        public SimpleDnsimpleResponse<LetsEncryptRenewalData>
            PurchaseLetsEncryptCertificateRenewal(
                long accountId, string domainName, long certificateId,
                LetsEncryptRenewal renewal)
        {
            var requestBuilder = Client.Http.RequestBuilder(
                LetsEncryptCertificateRenewalPath(accountId, domainName,
                    certificateId));
            requestBuilder.Method(Method.POST);
            requestBuilder.AddJsonPayload(renewal);

            return new SimpleDnsimpleResponse<LetsEncryptRenewalData>(
                Client.Http.Execute(requestBuilder.Request));
        }

        /// <summary>
        /// Issue a Let’s Encrypt certificate renewed with DNSimple.
        /// </summary>
        /// <param name="accountId">The account id</param>
        /// <param name="domainName">The domain name or id</param>
        /// <param name="certificateId">The certificate id</param>
        /// <param name="certificateRenewalId">The certificate renewal id</param>
        /// <returns>The certificate data issued in the renewal</returns>
        public SimpleDnsimpleResponse<Certificate>
            IssueLetsEncryptCertificateRenewal(long accountId,
                string domainName, long certificateId,
                long certificateRenewalId)
        {
            var requestBuilder =
                Client.Http.RequestBuilder(
                    IssueLetsEncryptCertificateRenewalPath(accountId,
                        domainName, certificateId, certificateRenewalId));
            requestBuilder.Method(Method.POST);

            return new SimpleDnsimpleResponse<Certificate>(
                Client.Http.Execute(requestBuilder.Request));
        }
    }

    /// <summary>
    /// Represents the letsencrypt renewal data.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct LetsEncryptRenewalData
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
    /// Represents the data we send to the server when renewing a letsencrypt
    /// certificate.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct LetsEncryptRenewal
    {
        public bool AutoRenew { get; set; }
    }

    /// <summary>
    /// Represents the certificate purchase order data.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct CertificateOrder
    {
        public int ContactId { get; set; }
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
    /// Represents the private key.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct PrivateKeyData
    {
        public string PrivateKey { get; set; }
    }

    /// <summary>
    /// Represents the <c>struct</c> containing a <c>List</c> of
    /// <c>CertificateData</c> objects.
    /// </summary>
    /// <see cref="List{T}"/>
    /// <see cref="CertificatesData"/>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct CertificatesData
    {
        /// <summary>
        /// The list of certificates.
        /// </summary>
        public List<Certificate> Certificates { get; }

        /// <summary>
        /// Creates a new <c>CertificatesData</c> object.
        /// </summary>
        /// <param name="json">The json payload containing the raw data.</param>
        public CertificatesData(JToken json) =>
            Certificates = DeserializeList(json);
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
    /// Represents a PEM encoded certificate.
    /// </summary>
    /// /// <see>https://developer.dnsimple.com/v2/certificates/#certificate-attributes</see>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy),
        ItemNullValueHandling = NullValueHandling.Ignore)]
    public struct PemEncodedCertificate
    {
        public string Server { get; set; }
        public string Root { get; set; }
        public List<string> Chain { get; set; }
    }
}