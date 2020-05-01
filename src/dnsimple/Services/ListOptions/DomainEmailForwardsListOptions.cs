namespace dnsimple.Services.ListOptions
{
    /// <summary>
    /// Defines the options you may want to send to list domain email forwards,
    /// such as pagination and sorting.
    /// </summary>
    /// <see cref="ListOptions"/>
    public class DomainEmailForwardsListOptions : ListOptions
    {
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
            AddSortCriteria(new Sort { Field = "id", Order = order});
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by from.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>DomainEmailForwardsListOptions</c></returns>
        public DomainEmailForwardsListOptions SortByFrom(Order order)
        {
            AddSortCriteria(new Sort{Field = "from", Order = order});
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by to.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>DomainEmailForwardsListOptions</c></returns>
        public DomainEmailForwardsListOptions SortByTo(Order order)
        {
            AddSortCriteria(new Sort{Field = "to", Order = order});
            return this;
        }
    }
}