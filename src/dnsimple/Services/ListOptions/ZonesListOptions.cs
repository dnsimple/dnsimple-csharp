namespace dnsimple.Services.ListOptions
{
    /// <summary>
    /// Defines the options you may want to send to list zones, such as
    /// pagination, sorting and filtering.
    /// </summary>
    /// <see cref="ListOptionsWithFiltering"/>
    public class ZonesListOptions : ListOptionsWithFiltering
    {
        /// <summary>
        /// Sets the name to be filtered by.
        /// </summary>
        /// <param name="name">The name we want to filter by.</param>
        /// <returns>The instance of the <c>ZonesListOptions</c></returns>
        public ZonesListOptions FilterByName(string name)
        {
            AddFilter(new Filter { Field = "name_like", Value = name });
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by id.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>ZonesListOptions</c></returns>
        /// <see cref="Order"/>
        public ZonesListOptions SortById(Order order)
        {
            AddSortCriteria(new Sort { Field = "id", Order = order });
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by name.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>ZonesListOptions</c></returns>
        /// <see cref="Order"/>
        public ZonesListOptions SortByName(Order order)
        {
            AddSortCriteria(new Sort { Field = "name", Order = order});
            return this;
        }
    }
}