using Newtonsoft.Json.Linq;

namespace dnsimple.Services
{
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

        protected SimpleDnsimpleResponse(JToken json) => 
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
        public T Data { get; protected set; }

        protected ListDnsimpleResponse() =>
            Data = new T();
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
        public T Data { get; protected set; }

        /// <summary>
        /// The <c>Pagination</c> object containing the pagination data
        /// </summary>
        /// <see cref="PaginationData"/>
        public PaginationData PaginationData { get; }

        protected PaginatedDnsimpleResponse(JToken response)
        {
            Data = new T();
            PaginationData = PaginationData.From(response);
        }
    }
}