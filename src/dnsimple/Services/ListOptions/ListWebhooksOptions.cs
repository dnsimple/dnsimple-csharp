namespace dnsimple.Services.ListOptions
{
    /// <summary>
    /// Defines the options you may want to send to list the webhooks for the
    /// account, such as pagination and sorting.
    /// </summary>
    /// <see cref="ListOptions"/>
    public class ListWebhooksOptions : ListOptions
    {
        
        /// <summary>
        /// Sets the order by which to sort by id.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of <c>ListWebhooksOptions</c></returns>
        public ListWebhooksOptions SortById(Order order)
        {
            AddSortCriteria(new Sort { Field = "id", Order = order });
            return this;
        }
    }
}