namespace dnsimple.Services.ListOptions
{
    /// <summary>
    /// Defines the options you may want to send to list domain email forwards,
    /// such as pagination and sorting.
    /// </summary>
    /// <see cref="ListOptions"/>
    public class DomainEmailForwardsListOptions : ListOptions
    {
        private const string IdSort = "id";
        private const string FromSort = "from";
        private const string ToSort = "to";

        /// <summary>
        /// Creates a new instance of <c>DomainEmailForwardsListOptions</c>
        /// </summary>
        public DomainEmailForwardsListOptions() =>
            Pagination = new Pagination();
        
        /// <summary>
        /// Sets the order by which to sort by id.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>DomainEmailForwardsListOptions</c></returns>
        public DomainEmailForwardsListOptions SortById(Order order)
        {
            AddSortCriteria(new Sort { Field = IdSort, Order = order});
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by from.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>DomainEmailForwardsListOptions</c></returns>
        public DomainEmailForwardsListOptions SortByFrom(Order order)
        {
            AddSortCriteria(new Sort{Field = FromSort, Order = order});
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by to.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>DomainEmailForwardsListOptions</c></returns>
        public DomainEmailForwardsListOptions SortByTo(Order order)
        {
            AddSortCriteria(new Sort{Field = ToSort, Order = order});
            return this;
        }
    }
}