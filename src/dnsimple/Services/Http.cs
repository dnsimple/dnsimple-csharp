using System.Net;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace dnsimple.Services
{
    /// <summary>
    /// Service used to interact with the API at a transport (HTTP) level.
    /// </summary>
    public class HttpService
    {
        private readonly RequestBuilder _builder;
        private IRestClient RestClient { get; }

        protected HttpService()
        {
        }

        /// <summary>
        /// Constructs the HTTP service by passing an instance of a
        /// <c>RestClient</c> and <c>RequestBuilder</c> objects.
        /// </summary>
        /// <param name="restClient">RestClient instance to be used.</param>
        /// <param name="builder">RequestBuilder instance to be used.</param>
        /// 
        /// <see cref="IRestClient"/>
        /// <see cref="RequestBuilder"/>
        public HttpService(IRestClient restClient, RequestBuilder builder)
        {
            RestClient = restClient;
            _builder = builder;
        }

        /// <summary>
        /// Creates a <c>RequestBuilder</c> with the path to the resource.
        /// </summary>
        /// <example>
        /// <code>
        ///     var builder = new RequestBuilder("/whoami");
        /// </code>
        /// </example>
        /// 
        /// <param name="path">Path to the resource</param>
        /// <returns><c>RequestBuilder</c> instance.</returns>
        public virtual RequestBuilder RequestBuilder(string path)
        {
            _builder.Reset();
            _builder.AddPath(path);
            return _builder;
        }

        /// <summary>
        /// Executes the request passed (GET, POST, etc.)
        /// </summary>
        /// <remarks>
        /// The <c>RestRequest</c> instance will have been build with a
        /// <c>RequestBuilder</c> and contains all the information
        /// (headers and parameters) needed to successfully issue the
        /// request to the server.
        /// </remarks>
        /// <param name="request"><c>RestRequest</c></param>
        /// <returns>
        /// A <c>JToken</c> object representing the JSON payload returned
        /// by the API call.
        /// </returns>
        /// <exception cref="DNSimpleException"></exception>
        /// <see cref="JToken"/>
        public virtual JToken Execute(IRestRequest request)
        {
            var restResponse = RestClient.Execute(request);

            if (!restResponse.IsSuccessful) HandleExceptions(restResponse);

            return JObject.Parse(restResponse.Content);
        }

        private static void HandleExceptions(IRestResponse restResponse)
        {
            var message = JObject.Parse(restResponse.Content)["message"].ToString();
            switch (restResponse.StatusCode)
            {
                case HttpStatusCode.BadGateway:
                    break;
                case HttpStatusCode.BadRequest:
                    break;
                case HttpStatusCode.Conflict:
                    break;
                case HttpStatusCode.ExpectationFailed:
                    break;
                case HttpStatusCode.Forbidden:
                    break;
                case HttpStatusCode.GatewayTimeout:
                    break;
                case HttpStatusCode.Gone:
                    break;
                case HttpStatusCode.HttpVersionNotSupported:
                    break;
                case HttpStatusCode.InternalServerError:
                    break;
                case HttpStatusCode.LengthRequired:
                    break;
                case HttpStatusCode.MethodNotAllowed:
                    break;
                case HttpStatusCode.NoContent:
                    break;
                case HttpStatusCode.NonAuthoritativeInformation:
                    break;
                case HttpStatusCode.NotAcceptable:
                    break;
                case HttpStatusCode.NotFound:
                    throw new NotFoundException(message);
                case HttpStatusCode.NotImplemented:
                    break;
                case HttpStatusCode.PaymentRequired:
                    break;
                case HttpStatusCode.PreconditionFailed:
                    break;
                case HttpStatusCode.ProxyAuthenticationRequired:
                    break;
                case HttpStatusCode.RequestedRangeNotSatisfiable:
                    break;
                case HttpStatusCode.RequestEntityTooLarge:
                    break;
                case HttpStatusCode.RequestTimeout:
                    break;
                case HttpStatusCode.RequestUriTooLong:
                    break;
                case HttpStatusCode.ServiceUnavailable:
                    throw new DNSimpleException(message);
                case HttpStatusCode.Unauthorized:
                    throw new AuthenticationException(message);
                case HttpStatusCode.UnsupportedMediaType:
                    break;
                case HttpStatusCode.Unused:
                    break;
                case HttpStatusCode.UpgradeRequired:
                    break;
                case HttpStatusCode.UseProxy:
                    break;
                default:
                    throw new DNSimpleException(message);
            }
        }
    }
}
