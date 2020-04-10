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
        /// <see cref="JToken"/>
        public virtual JToken Execute(RestRequest request)
        {
            // TODO: Proper Exception handling has to happen here (we are currently swallowing every issue and would not know what's going on unless we debug)
            // Basically introducing a DNSimpleException object with it's inheritors if needed.
            return JObject.Parse(RestClient.Execute(request).Content);
        }
    }
}