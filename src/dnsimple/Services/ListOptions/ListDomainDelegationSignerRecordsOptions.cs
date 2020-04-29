namespace dnsimple.Services.ListOptions
{
    /// <summary>
    /// Defines the options you may want to send to list domain delegation
    /// signer records, such as pagination and sorting.
    /// </summary>
    /// <see cref="ListOptions"/>
    public class ListDomainDelegationSignerRecordsOptions : ListOptions {

        /// <summary>
        /// Sets the order by which to sort by id.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>ListDomainDelegationSignerRecordsOptions</c></returns>
        public ListDomainDelegationSignerRecordsOptions SortById(Order order)
        {
            AddSortCriteria(new Sort {Field = "id", Order = order });
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by created at.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>ListDomainDelegationSignerRecordsOptions</c></returns>
        public ListDomainDelegationSignerRecordsOptions SortByCreatedAt(
            Order order)
        {
            AddSortCriteria(new Sort{Field = "created_at", Order = order });
            return this;
        }
    }
}