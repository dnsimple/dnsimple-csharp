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
        private RestClientWrapper ClientWrapper { get; }

        protected HttpService() {}

        /// <summary>
        /// Constructs the HTTP service by passing an instance of a
        /// <c>RestClientWrapper</c> and <c>RequestBuilder</c> objects.
        /// </summary>
        /// <param name="clientWrapper">RestClientWrapper instance to be used.</param>
        /// <param name="builder">RequestBuilder instance to be used.</param>
        public HttpService(RestClientWrapper clientWrapper, RequestBuilder builder)
        {
            ClientWrapper = clientWrapper;
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
        /// The <c>RestRequest</c> instance will have been built with a
        /// <c>RequestBuilder</c> and contains all the information
        /// (headers and parameters) needed to successfully issue the
        /// request to the server.
        /// </remarks>
        /// <param name="request"><c>RestRequest</c></param>
        /// <returns>
        /// The <c>RestResponse</c> returned by the API call.
        /// </returns>
        public virtual RestResponse Execute(RestRequest request)
        {
            var response = ClientWrapper.RestClient.Execute(request);

            if (!response.IsSuccessful) HandleExceptions(response);

            return response;
        }

        internal static void HandleExceptions(RestResponse restResponse)
        {
            var error = JObject.Parse(restResponse.Content);
            var message = error["message"]?.ToString();

            switch (restResponse.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                    if(error["errors"] != null)
                        throw new DnsimpleValidationException(error);
                    throw new DnsimpleException(message);
                case HttpStatusCode.NotFound:
                    throw new NotFoundException(message);
                case HttpStatusCode.PaymentRequired:
                    throw new DnsimpleException(message);
                case HttpStatusCode.PreconditionFailed:
                    throw new DnsimpleException(message);
                case (HttpStatusCode) 429 :
                    throw new DnsimpleException(message);
                case HttpStatusCode.Unauthorized:
                    throw new AuthenticationException(message);
                case HttpStatusCode.InternalServerError:
                    throw new DnsimpleException(message);
                case HttpStatusCode.NotImplemented:
                    throw new DnsimpleException(message);
                case HttpStatusCode.ServiceUnavailable:
                    throw new DnsimpleException(message);
                case HttpStatusCode.GatewayTimeout:
                    throw new DnsimpleException(message);
                default:
                    throw new DnsimpleException(message);
            }
        }
    }
}
