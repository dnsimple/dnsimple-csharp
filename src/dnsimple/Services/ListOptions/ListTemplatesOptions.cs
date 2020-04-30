namespace dnsimple.Services.ListOptions
{
    /// <summary>
    /// Defines the options you may want to send to list the templates for the
    /// account, such as pagination and sorting.
    /// </summary>
    /// <see cref="ListOptions"/>
    public class ListTemplatesOptions : ListOptions
    {
        /// <summary>
        /// Sets the order by which to sort by id.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of <c>ListTemplatesOptions</c></returns>
        public ListTemplatesOptions SortById(Order order)
        {
            AddSortCriteria(new Sort { Field = "id", Order = order });
            return this;
        }
        
        /// <summary>
        /// Sets the order by which to sort by name.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of <c>ListTemplatesOptions</c></returns>
        public ListTemplatesOptions SortByName(Order order)
        {
            AddSortCriteria(new Sort { Field = "name", Order = order });
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by sid.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of <c>ListTemplatesOptions</c></returns>
        public ListTemplatesOptions SortBySid(Order order)
        {
            AddSortCriteria(new Sort { Field = "sid", Order =  order });
            return this;
        }
    }
}