using System;
using RestSharp;
using RestSharp.Authenticators;

namespace dnsimple
{
    /// <summary>
    /// Wraps the <c>RestSharp.RestClient</c>.
    /// </summary>
    /// <see cref="RestSharp.RestClient"/>
    public class RestClientWrapper
    {
        private RestClient _restClient;
        private IAuthenticator _authenticator;
        private Uri _baseUrl;
        private string _userAgent;

        /// <summary>
        /// The instance of the <c>RestSharp.RestClient</c>.
        /// </summary>
        /// <see cref="RestSharp.RestClient"/>
        public virtual RestClient RestClient => _restClient ??= Build();

        /// <summary>
        /// Constructs a new <c>RestClientWrapper</c>
        /// </summary>
        public RestClientWrapper()
        {
        }

        /// <summary>
        /// Adds an <c>Authenticator</c> to the underlying <c>RestClient</c>.
        /// </summary>
        /// <remarks>
        /// Authenticators are used to interact with the DNSimple API. They can
        /// be either HTTPBasic (email and password) or OAuth2 tokens.
        /// </remarks>
        /// <param name="credentials">The credentials containing the authenticator to be used</param>
        /// <see cref="ICredentials"/>
        /// <see cref="RestSharp.Authenticators.IAuthenticator"/>
        public virtual void AddAuthenticator(ICredentials credentials)
        {
            _authenticator = credentials.Authenticator;
            _restClient = null;
        }

        internal void SetBaseUrl(Uri baseUrl)
        {
            _baseUrl = baseUrl;
            _restClient = null;
        }

        internal void SetUserAgent(string userAgent)
        {
            _userAgent = userAgent;
            _restClient = null;
        }

        private RestClient Build()
        {
            var options = new RestClientOptions
            {
                Authenticator = _authenticator,
                UserAgent = _userAgent,
            };
            if (_baseUrl != null) options.BaseUrl = _baseUrl;
            return new RestClient(options);
        }
    }
}
