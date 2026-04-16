using System.Text;
using dnsimple.Services;
using RestSharp.Authenticators;

namespace dnsimple
{
    /// <summary>
    /// Interface to the credentials object.
    ///
    /// Credentials are used to pass into the API either HTTPBasic credentials
    /// (in the form of name and password) or OAuth2 (in the form of a TOKEN
    /// obtained during the Authentication dance).
    /// </summary>
    public interface ICredentials
    {
        IAuthenticator Authenticator { get; }
    }

    /// <summary>
    /// Credentials used to authenticate with name and password.
    /// </summary>
    /// <code>
    ///     var credentials = new BasicHttpCredentials("example-account@example.com", "secret");
    /// </code>
    public readonly struct BasicHttpCredentials : ICredentials
    {
        public IAuthenticator Authenticator { get; }

        public BasicHttpCredentials(string name, string password) =>
            Authenticator = new HttpBasicAuthenticator(name, password, Encoding.UTF8);
    }

    /// <summary>
    /// Credentials used to authorize operations using an OAuth2 Token
    /// </summary>
    /// <code>
    ///     var credentials = new Oauth2Credentials("TOKEN");
    /// </code>
    /// <seealso cref="OAuth2Service"/>
    public readonly struct OAuth2Credentials : ICredentials
    {
        public IAuthenticator Authenticator { get; }

        public OAuth2Credentials(string token) =>
            Authenticator = new RestSharp.Authenticators.OAuth2.OAuth2AuthorizationRequestHeaderAuthenticator(token, "Bearer");
    }
}
