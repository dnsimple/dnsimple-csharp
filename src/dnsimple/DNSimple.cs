using System;
using dnsimple.Services;
using RestSharp;

namespace dnsimple
{
    /// <summary>
    /// Client for the DNSimple API.
    /// </summary>
    /// <see>
    ///     <cref>https://developer.dnsimple.com/</cref>
    /// </see>
    /// <see>
    ///     <cref>https://developer.dnsimple.com/sandbox/</cref>
    /// </see>
    /// 
    /// <example>
    /// Default (Production)
    ///     <code>
    ///     using dnsimple;
    /// 
    ///     var client = new Client();
    ///     var credentials = new OAuth2Credentials("...");
    ///     client.AddCredentials(credentials);
    /// </code>
    /// </example>
    /// 
    /// <example>
    /// Custom Base URL (Sandbox)
    ///     <code>
    ///     using dnsimple;
    /// 
    ///     var client = new Client();
    ///     client.ChangeBaseUrlTo("https://api.sandbox.dnsimple.com");
    ///     var credentials = new OAuth2Credentials("...");
    ///     client.AddCredentials(credentials);
    /// </code>
    /// </example>
    /// 
    /// <remarks>
    /// Gives access to all the services the API provides.
    /// </remarks>
    public interface IClient
    {
        /// <summary>
        /// Base URL for API calls.
        /// </summary>
        string BaseUrl { get; }
        /// <summary>
        /// Current API version
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Instance of the <c>AccountsService</c>
        /// </summary>
        /// <see cref="AccountsService"/>
        /// <see>https://developer.dnsimple.com/v2/accounts/</see>
        AccountsService Accounts { get; }
        
        /// <summary>
        /// Instance of the <c>CertificatesService</c>
        /// </summary>
        /// <see cref="CertificatesService"/>
        /// <see>https://developer.dnsimple.com/v2/certificates/</see>
        CertificatesService Certificates { get; }
        
        /// <summary>
        /// Instance of the <c>DomainsService</c>
        /// <see cref="DomainsService"/>
        /// <see>https://developer.dnsimple.com/v2/domains/</see>
        /// </summary>
        DomainsService Domains { get; }

        /// <summary>
        /// Instance of the <c>HttpService</c>
        /// </summary>
        /// <see cref="HttpService"/>
        HttpService Http { get; }

        /// <summary>
        /// Instance of the <c>IdentityService</c>
        /// </summary>
        /// <see cref="IdentityService"/>
        /// <see>https://developer.dnsimple.com/v2/identity/</see>
        IdentityService Identity { get; }

        /// <summary>
        /// Instance of the <c>OAuth2Service</c>
        /// </summary>
        /// <see cref="OAuth2Service"/>
        OAuth2Service OAuth { get; }
        
        /// <summary>
        /// Instance of the <c>RegistrarService</c>
        /// </summary>
        /// <see cref="RegistrarService"/>
        RegistrarService Registrar { get; }
        
        /// <summary>
        /// Instance of the <c>TldsService</c>
        /// </summary>
        /// <see cref="TldsService"/>
        /// <see>https://developer.dnsimple.com/v2/tlds/</see>
        TldsService Tlds { get; }
        
        /// <summary>
        /// Instance of the <c>ZonesService</c>
        /// <see cref="ZonesService"/>
        /// <see>https://developer.dnsimple.com/v2/zones/</see>
        /// </summary>
        ZonesService Zones { get; }

        /// <summary>
        /// Changes the base URL to be used (i.e. to the sandbox environment).
        /// </summary>
        /// <example>
        ///     <code>
        ///         client.ChangeBaseUrlTo("https://api.sandbox.dnsimple.com");
        ///     </code>
        /// </example>
        /// <param name="baseUrl"></param>
        void ChangeBaseUrlTo(string baseUrl);
        
        /// <summary>
        /// Returns the URL to the API including the version
        /// </summary>
        /// <example>
        ///    https://api.dnsimple.com/v2/
        /// </example>
        /// <returns>A string representation of the URL to the API</returns>
        string VersionedBaseUrl();
        
        /// <summary>
        /// Adds the credentials to be used to Authenticate with the API.
        /// </summary>
        /// <param name="credentials">
        ///     <see cref="ICredentials"/>
        /// </param>
        /// <seealso cref="BasicHttpCredentials"/>
        /// <seealso cref="OAuth2Credentials"/>
        void AddCredentials(ICredentials credentials);
    }

    /// <summary>
    /// Client for the DNSimple API.
    /// </summary>
    /// <see>
    ///     <cref>https://developer.dnsimple.com/</cref>
    /// </see>
    /// <see>
    ///     <cref>https://developer.dnsimple.com/sandbox/</cref>
    /// </see>
    /// <example>
    /// Default (Production)
    ///     <code>
    ///     using dnsimple;
    /// 
    ///     var client = new Client();
    ///     var credentials = new OAuth2Credentials("...");
    ///     client.AddCredentials(credentials);
    /// </code>
    /// </example>
    /// <example>
    /// Custom Base URL (Sandbox)
    ///     <code>
    ///     using dnsimple;
    /// 
    ///     var client = new Client();
    ///     client.ChangeBaseUrlTo("https://api.sandbox.dnsimple.com");
    ///     var credentials = new OAuth2Credentials("...");
    ///     client.AddCredentials(credentials);
    /// </code>
    /// </example>
    /// <remarks>
    ///     Gives access to all the services the API provides.
    /// </remarks>
    public class Client : IClient
    {
        /// <summary>
        /// Base URL for API calls.
        ///
        /// It defaults to https://api.dnsimple.com.
        /// For testing purposes use https://api.sandbox.dnsimple.com
        /// </summary>
        /// <see>
        ///     <cref>https://developer.dnsimple.com/sandbox/</cref>
        /// </see>
        public string BaseUrl { get; private set; } =
            "https://api.dnsimple.com";

        /// <summary>
        /// Current API version
        /// </summary>
        public string Version { get; } = "v2";

        /// <summary>
        /// Instance of the <c>RestClientWrapper</c>
        /// </summary>
        /// <see cref="RestClientWrapper"/>
        private RestClientWrapper RestClientWrapper { get; }

        /// <inheritdoc />
        public AccountsService Accounts { get; private set; }

        public CertificatesService Certificates { get; private set; }
        
        /// <inheritdoc/>
        public DomainsService Domains { get; private set; }

        /// <inheritdoc />
        public HttpService Http { get; private set; }

        /// <inheritdoc />
        public IdentityService Identity { get; private set; }

        /// <inheritdoc />
        public OAuth2Service OAuth { get; private set; }
        
        public RegistrarService Registrar { get; private set; }
        
        public TldsService Tlds { get; private set; }

        public ZonesService Zones { get; private set; }

        /// <summary>
        /// Constructs a new Client initializing a new (default)
        /// <c>RestClientWrapper</c>.
        /// </summary>
        /// <see cref="RestClientWrapper"/>
#pragma warning disable 618
        public Client() : this(new RestClientWrapper())
#pragma warning restore 618
        {
        }

        /// <summary>
        /// Constructs a new client by passing a RestClientWrapper.
        /// </summary>
        /// <param name="restClientWrapper"></param>
        [Obsolete(
            "Please don't use this method, use the default constructor instead.")]
        public Client(RestClientWrapper restClientWrapper)
        {
            RestClientWrapper = restClientWrapper;
            InitializeRestClient();
            InitializeServices();
        }

        /// <summary>
        /// Changes the base URL to be used (i.e. to the sandbox environment).
        /// </summary>
        /// <example>
        ///     <code>
        ///         client.ChangeBaseUrlTo("https://api.sandbox.dnsimple.com");
        ///     </code>
        /// </example>
        /// <param name="baseUrl"></param>
        public void ChangeBaseUrlTo(string baseUrl)
        {
            BaseUrl = baseUrl;
            RestClientWrapper.RestClient.BaseUrl = new Uri(VersionedBaseUrl());
        }
        
        /// <summary>
        /// Returns the URL to the API including the version
        /// </summary>
        /// <remarks>
        ///    https://api.dnsimple.com/v2/
        /// </remarks>
        /// <returns>A string representation of the URL to the API</returns>
        public string VersionedBaseUrl()
        {
            return $"{BaseUrl}/{Version}/";
        }

        /// <summary>
        /// Adds the credentials to be used to Authenticate with the API.
        /// </summary>
        /// <param name="credentials">
        ///     <see cref="ICredentials"/>
        /// </param>
        /// <seealso cref="BasicHttpCredentials"/>
        /// <seealso cref="OAuth2Credentials"/>
        public void AddCredentials(ICredentials credentials) => 
            RestClientWrapper.AddAuthenticator(credentials);

        /// <summary>
        /// Initializes the <c>RestClient</c> contained in the
        /// <c>RestClientWrapper</c>.
        /// </summary>
        /// <see cref="RestClientWrapper"/>
        /// <see cref="RestClient"/>
        private void InitializeRestClient()
        {
            RestClientWrapper.RestClient.BaseUrl = new Uri(VersionedBaseUrl());
            RestClientWrapper.RestClient.UseJson();
        }

        /// <summary>
        /// Initializes all the services offered by the current version of
        /// this API.
        /// </summary>
        /// <see cref="IdentityService"/>
        /// <see cref="HttpService"/>
        /// <see cref="OAuth2Service"/>
        private void InitializeServices()
        {
            Accounts = new AccountsService(this);
            Certificates = new CertificatesService(this);
            Domains = new DomainsService(this);
            Http = new HttpService(RestClientWrapper.RestClient,
                new RequestBuilder());
            Identity = new IdentityService(this);
            OAuth = new OAuth2Service(Http);
            Registrar = new RegistrarService(this);
            Tlds = new TldsService(this);
            Zones = new ZonesService(this);
        }
    }
}