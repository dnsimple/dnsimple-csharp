using System.Collections.Generic;
using System.Linq;
using dnsimple.Services.ListOptions;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace dnsimple.Services
{
    public abstract class ServiceBase
    {
        private IClient Client { get; }

        /// <summary>
        /// Creates a new instance of a Service by passing an
        /// instance of the DNSimple <c>IClient</c>
        /// </summary>
        /// <param name="client">An instance of the <c>IClient</c></param>
        /// <see cref="IClient"/>
        protected ServiceBase(IClient client) =>
            Client = client;

        protected RequestBuilder BuildRequestForPath(string path)
        {
            return Client.Http.RequestBuilder(path);
        }

        protected IRestResponse Execute(RestRequest request)
        {
            return Client.Http.Execute(request);
        }

        protected static void AddListOptionsToRequest(
            ListOptions.ListOptions options,
            ref RequestBuilder requestBuilder)
        {
            if (options == null) return;
            if (options.HasSortingOptions())
                requestBuilder.AddParameter(options.UnpackSorting());
            if (options.HasFilterOptions())
                requestBuilder.AddParameters(options.UnpackFilters());
            if (!options.Pagination.IsDefault())
                requestBuilder.AddParameters(options.UnpackPagination());
        }
    }

    public abstract class Response
    {
        public readonly IList<Parameter> Headers;
        public readonly int RateLimit;
        public readonly int RateLimitRemaining;
        public readonly int RateLimitReset;

        public Response(IRestResponse response)
        {
            Headers = response.Headers;
            RateLimit = int.Parse(ExtractValueFromHeader("X-RateLimit-Limit"));
            RateLimitRemaining = int.Parse(ExtractValueFromHeader("X-RateLimit-Remaining"));
            RateLimitReset = int.Parse(ExtractValueFromHeader("X-RateLimit-Reset"));
        }

        private string ExtractValueFromHeader(string headerName)
        {
            return (string) Headers.Where(header => header.Name.Equals(headerName))
                .First().Value;
        }
    }

    /// <summary>
    /// Represents an empty response (204 No Content) from a call to the
    /// DNSimple API.
    /// </summary>
    public class EmptyDnsimpleResponse : Response
    {
        public EmptyDnsimpleResponse(IRestResponse response) : base(response)
        {
        }
    }

    /// <summary>
    /// Represents the most basic response from a call to the DNSimple API.
    /// </summary>
    /// <typeparam name="T">The Data object type contained in the response</typeparam>
    public class SimpleDnsimpleResponse<T> : Response
    {
        /// <summary>
        /// Represents the <c>struct</c> containing the data.
        /// </summary>
        public T Data { get; protected set; }

        public SimpleDnsimpleResponse(IRestResponse response) : base(response)
        {
            Data = JsonTools<T>.DeserializeObject("data",
                JObject.Parse(response.Content));
        }
    }


    /// <summary>
    /// Represents a response from a call to the DNSimple API containing a list
    /// of objects.
    /// </summary>
    /// <typeparam name="T">The Data object type contained in the response</typeparam>
    public class ListDnsimpleResponse<T> : Response where T : new()
    {
        /// <summary>
        /// Represents the <c>struct</c> containing the data.
        /// </summary>
        public List<T> Data { get; }

        public ListDnsimpleResponse(IRestResponse response) : base(response)
        {
            Data = JsonTools<T>.DeserializeList(
                JObject.Parse(response.Content));
        }
    }

    /// <summary>
    /// Represents a response from a call to the DNSimple API containing a list
    /// and a pagination.
    /// </summary>
    /// <typeparam name="T">The Data object type contained in the response</typeparam>
    /// <see cref="Pagination"/>
    public class PaginatedDnsimpleResponse<T> : Response where T : new()
    {
        /// <summary>
        /// Represents the <c>struct</c> containing the data.
        /// </summary>
        public List<T> Data { get; }

        /// <summary>
        /// The <c>Pagination</c> object containing the pagination data
        /// </summary>
        /// <see cref="Pagination"/>
        public Pagination Pagination { get; }

        public PaginatedDnsimpleResponse(IRestResponse response) : base(response)
        {
            var json = JObject.Parse(response.Content);

            Data = JsonTools<T>.DeserializeList(json);
            Pagination = Pagination.From(json);
        }
    }
}