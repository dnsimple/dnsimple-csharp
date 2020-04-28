using System.Collections.Generic;
using Newtonsoft.Json.Linq;

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
    /// <summary>
    /// Represents the most basic response from a call to the DNSimple API.
    /// </summary>
    /// <typeparam name="T">The Data object type contained in the response</typeparam>
    public class SimpleDnsimpleResponse<T>
    {
        /// <summary>
        /// Represents the <c>struct</c> containing the data.
        /// </summary>
        public T Data { get; protected set; }

        public SimpleDnsimpleResponse(JToken json) => 
            Data = JsonTools<T>.DeserializeObject("data", json);
    }
    

    /// <summary>
    /// Represents a response from a call to the DNSimple API containing a list
    /// of objects.
    /// </summary>
    /// <typeparam name="T">The Data object type contained in the response</typeparam>
    public class ListDnsimpleResponse<T> where T : new()
    {
        /// <summary>
        /// Represents the <c>struct</c> containing the data.
        /// </summary>
        public List<T> Data { get; }

        public ListDnsimpleResponse(JToken response) =>
            Data = JsonTools<T>.DeserializeList(response);
    }

    /// <summary>
    /// Represents a response from a call to the DNSimple API containing a list
    /// and a pagination.
    /// </summary>
    /// <typeparam name="T">The Data object type contained in the response</typeparam>
    /// <see cref="PaginationData"/>
    public class PaginatedDnsimpleResponse<T> where T : new()
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

        public PaginatedDnsimpleResponse(JToken response)
        {
            Data = JsonTools<T>.DeserializeList(response);
            PaginationData = PaginationData.From(response);
        }
    }
}