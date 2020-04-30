namespace dnsimple.Services.ListOptions
{
    /// <summary>
    /// Defines the options you may want to send to list the services available,
    /// such as pagination and sorting.
    /// </summary>
    /// <see cref="ListOptions"/>
    public class ListServicesOptions : ListOptions
    {
        /// <summary>
        /// Sets the order by which to sort by id.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of <c>ListServicesOptions</c></returns>
        public ListServicesOptions SortById(Order order)
        {
            AddSortCriteria(new Sort { Field = "id", Order = order });
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by sid.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of <c>ListServicesOptions</c></returns>
        public ListServicesOptions SortBySid(Order order)
        {
            AddSortCriteria(new Sort { Field = "sid", Order = order});
            return this;
        }
    }
}