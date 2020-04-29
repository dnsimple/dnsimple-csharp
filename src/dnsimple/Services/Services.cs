using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace dnsimple.Services
{
    public abstract class Service
    {
        
        protected IClient Client { get; }

        /// <summary>
        /// Creates a new instance of a Service by passing an
        /// instance of the DNSimple <c>IClient</c>
        /// </summary>
        /// <param name="client">An instance of the <c>IClient</c></param>
        /// <see cref="IClient"/>
        protected Service(IClient client) =>
            Client = client;
    }
    
    public abstract class Response
    {
        public IList<Parameter> Headers;
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

        public SimpleDnsimpleResponse(IRestResponse response)
        {
            Headers = response.Headers;
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

        public ListDnsimpleResponse(IRestResponse response)
        {
            Headers = response.Headers;
            Data = JsonTools<T>.DeserializeList(JObject.Parse(response.Content));
        }
    }

    /// <summary>
    /// Represents a response from a call to the DNSimple API containing a list
    /// and a pagination.
    /// </summary>
    /// <typeparam name="T">The Data object type contained in the response</typeparam>
    /// <see cref="PaginationData"/>
    public class PaginatedDnsimpleResponse<T> : Response where T : new()
    {
        /// <summary>
        /// Represents the <c>struct</c> containing the data.
        /// </summary>
        public List<T> Data { get; }

        /// <summary>
        /// The <c>Pagination</c> object containing the pagination data
        /// </summary>
        /// <see cref="PaginationData"/>
        public PaginationData PaginationData { get; }

        public PaginatedDnsimpleResponse(IRestResponse response)
        {
            var json = JObject.Parse(response.Content);
            
            Headers = response.Headers;
            Data = JsonTools<T>.DeserializeList(json);
            PaginationData = PaginationData.From(json);
        }
    }
}