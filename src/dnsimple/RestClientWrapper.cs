using RestSharp;

namespace dnsimple
{
    /// <summary>
    /// Wraps the <c>RestSharp.RestClient</c>.
    /// </summary>
    /// <see cref="RestSharp.RestClient"/>
    public class RestClientWrapper
    {
        /// <summary>
        /// The instance of the <c>RestSharp.RestClient</c>.
        /// </summary>
        /// <see cref="RestSharp.RestClient"/>
        public RestClient RestClient { get; }

        /// <summary>
        /// Constructs a new <c>RestClientWrapper</c>
        /// </summary>
        public RestClientWrapper() : this(new RestClient())
        {
        }

        /// <summary>
        /// Adds an <c>Authenticator</c> to the <c>RestSharp.RestClient</c>.
        /// </summary>
        /// <remarks>
        /// Authenticators are used to interact with the DNSimple API. They can
        /// be either HTTPBasic (email and password) or OAuth2 tokens.
        /// </remarks>
        /// <param name="credentials">The credentials containing the authenticator to be used</param>
        /// <see cref="ICredentials"/>
        /// <see cref="RestSharp.Authenticators.IAuthenticator"/>
        public virtual void AddAuthenticator(ICredentials credentials)
            => RestClient.Authenticator = credentials.Authenticator;

        private RestClientWrapper(RestClient restClient)
            => RestClient = restClient;
    }
}