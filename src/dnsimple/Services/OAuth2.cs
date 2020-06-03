using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;

namespace dnsimple.Services
{
    /// <summary>
    /// The <c>OAuth2Service</c> handles the <c>OAuth2</c> interaction with the
    /// DNSimple API.
    /// </summary>
    /// <see>https://developer.dnsimple.com/v2/oauth/</see>
    public class OAuth2Service
    {
        private HttpService Http { get; }

        /// <summary>
        /// Constructs a new instance of <c>OAuth2Service</c> by passing an
        /// instance of <c>HttpService</c>.
        /// </summary>
        /// <param name="http"></param>
        public OAuth2Service(HttpService http) => Http = http;

        /// <summary>
        /// This method is to be called when you want to get a <c>TOKEN</c> to
        /// be used during your interaction with the DNSimple API.
        /// </summary>
        /// <example>
        ///     <code>
        ///         var client = new Client();
        ///         var authArguments = new Dictionary<!--<OAuthParams, string>-->
        ///         {
        ///             {OAuthParams.ClientId, "1"},
        ///             {OAuthParams.ClientSecret, "secret"},
        ///             {OAuthParams.Code, "code-from-call-in-first-step-of-oauth-dance"},
        ///             {OAuthParams.State, "state"},
        ///             {OAuthParams.RedirectUri, "/callback/uri"}
        ///         };
        ///         var accessTokenData = client.OAuth.ExchangeAuthorizationForToken(authArguments);
        ///         Console.WriteLine("The Access Token is: {0}", accessTokenData.AccessToken);
        ///     </code>
        /// </example>
        /// 
        /// <param name="arguments">A <c>Dictionary</c> of <c>OAuthParams</c>
        /// and <c>string</c> elements.</param>
        /// 
        /// <returns>The <c>AccessTokenData</c> object containing the access
        /// token to be used in subsequent calls of the Api</returns>
        /// 
        /// <see>https://developer.dnsimple.com/v2/oauth/</see>
        public AccessToken ExchangeAuthorizationForToken(Dictionary<OAuthParams, string> arguments)
        {
            var request = BuildRequest("/oauth/access_token", arguments);
            return JObject.Parse(Http.Execute(request).Content).ToObject<AccessToken>();
        }

        private RestRequest BuildRequest(string path, IReadOnlyDictionary<OAuthParams, string> arguments)
        {
            var builder = Http.RequestBuilder(path);
            builder.Method(Method.POST);
            builder.AddHeaders(Headers());
            builder.AddParameters(PostParameters(arguments));
            return builder.Request;
        }

        private static IEnumerable<KeyValuePair<string, string>> PostParameters(
            IReadOnlyDictionary<OAuthParams, string> arguments)
        {
            return new Collection<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("client_id",
                    arguments[OAuthParams.ClientId]),
                new KeyValuePair<string, string>("client_secret",
                    arguments[OAuthParams.ClientSecret]),
                new KeyValuePair<string, string>("grant_type",
                    "authorization_code"),
                new KeyValuePair<string, string>("code",
                    arguments[OAuthParams.Code]),
                new KeyValuePair<string, string>("redirect_uri",
                    arguments[OAuthParams.RedirectUri]),
                new KeyValuePair<string, string>("state",
                    arguments[OAuthParams.State])
            };
        }

        private static Collection<KeyValuePair<string, string>> Headers()
        {
            return new Collection<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Accept", "application/json"),
                new KeyValuePair<string, string>("Content-Type", "application/x-www-form-urlencoded")
            };
        }
    }

    /// <summary>
    /// Represents the <c>AccessTokenData</c> <c>struct</c> returned by the
    /// server.
    /// </summary>
    [JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
    public struct AccessToken
    {
        [JsonProperty("access_token")]
        public string Token       { get; set; }
        public string TokenType   { get; set; }
        public string Scope       { get; set; }
        public long AccountId     { get; set; }
    }

    /// <summary>
    /// Represents the <c>OAuthParams</c> that can (and have to) be used when
    /// issuing a new <c>AccessToken</c> from the API.
    /// </summary>
    public enum OAuthParams
    {
        ClientId,
        ClientSecret,
        Code,
        State,
        RedirectUri
    }
}
