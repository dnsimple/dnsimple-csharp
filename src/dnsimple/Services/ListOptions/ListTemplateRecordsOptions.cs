namespace dnsimple.Services.ListOptions
{
    /// <summary>
    /// Defines the options you may want to send to list the template records
    /// for the account, such as pagination and sorting.
    /// </summary>
    /// <see cref="ListOptions"/>
    public class ListTemplateRecordsOptions : ListOptions
    {
        /// <summary>
        /// Sets the order by which to sort by id.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of <c>ListTemplateRecordsOptions</c></returns>
        public ListTemplateRecordsOptions SortById(Order order)
        {
            AddSortCriteria(new Sort { Field = "id", Order = order });
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by name.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of <c>ListTemplateRecordsOptions</c></returns>
        public ListTemplateRecordsOptions SortByName(Order order)
        {
            AddSortCriteria(new Sort { Field = "name", Order = order });
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by content.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of <c>ListTemplateRecordsOptions</c></returns>
        public ListTemplateRecordsOptions SortByContent(Order order)
        {
            AddSortCriteria(new Sort { Field = "content", Order = order });
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by type.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of <c>ListTemplateRecordsOptions</c></returns>
        public ListTemplateRecordsOptions SortByType(Order order)
        {
            AddSortCriteria(new Sort {Field = "type", Order = order});
            return this;
        }
    }
}