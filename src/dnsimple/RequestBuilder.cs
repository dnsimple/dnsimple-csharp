using System.Collections.Generic;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using RestSharp;

namespace dnsimple
{
    /// <summary>
    /// The <c>RequestBuilder</c> helps building the <c>RestRequest</c> instances
    /// used to issue commands to the the DNSimple API.
    /// </summary>
    public class RequestBuilder
    {
        /// <summary>
        /// Represents the <c>RestRequest</c> we will be issuing. 
        /// </summary>
        public RestRequest Request { get; private set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public RequestBuilder()
        {
        }

        /// <summary>
        /// Builds a new <c>RequestBuilder</c> for the URI (endpoint).
        /// </summary>
        /// <param name="path">The endpoint we want to issue the request to</param>
        /// <example>
        ///     <code>
        ///         var builder = new RequestBuilder("/whoami");
        ///     </code>
        /// </example>
        public RequestBuilder(string path) => 
            AddPath(path);
        
        /// <summary>
        /// Adds headers to the request.
        /// </summary>
        /// <param name="headers">The headers we want to add to the request.</param>
        public void AddHeaders(Collection<KeyValuePair<string, string>> headers) => 
            Request.AddHeaders(headers);

        /// <summary>
        /// Adds parameters to the request.
        /// </summary>
        /// <param name="parameters">The parameters we want to add to the request.</param>
        public void AddParameters(
            IEnumerable<KeyValuePair<string, string>> parameters)
        {
            foreach (var pair in parameters)
            {
                Request.AddParameter(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// Adds a JSON payload to the body of the request.
        /// </summary>
        /// <param name="payload">The object to be serialized and send in the
        /// body of the request.</param>
        public void AddJsonPayload(object payload)
        {
            Request.AddJsonBody(JsonConvert.SerializeObject(payload));
        }

        /// <summary>
        /// Sets the HTTP method to be used.
        /// </summary>
        /// <param name="method"></param>
        /// <see cref="RestSharp.Method"/>
        public void Method(Method method) => 
            Request.Method = method;

        /// <summary>
        /// If the <c>RequestBuilder</c> was created with the default constructor
        /// we can set the path with this method.
        /// </summary>
        /// <param name="path"></param>
        /// <example>
        ///     <code>
        ///         var builder = new RequestBuilder();
        ///         builder.AddPath("/whoami");
        ///     </code>
        /// </example>
        public void AddPath(string path) => 
            Request = new RestRequest(path, DataFormat.Json);

        /// <summary>
        /// Resets the <c>RequestBuilder</c> emptying the <c>Request</c> contained.
        /// </summary>
        /// <returns>The instance of the <c>RequestBuilder</c></returns>
        public RequestBuilder Reset()
        {
            Request = null;
            return this;
        }

        public void Pagination(int perPage, int page)
        {
            var pagination = new Collection<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("per_page", perPage.ToString()),
                new KeyValuePair<string, string>("page", page.ToString())
            };
            AddParameters(pagination);
        }
    }
}