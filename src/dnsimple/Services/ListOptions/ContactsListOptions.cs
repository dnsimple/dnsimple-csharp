namespace dnsimple.Services.ListOptions
{
    /// <summary>
    /// Defines the options you may want to send to list contacts, such as
    /// pagination and sorting
    /// </summary>
    /// <see cref="ListOptions"/>
    public class ContactsListOptions : ListOptions
    {
        /// <summary>
        /// Sets the order by which to sort by id.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of <c>ContactsListOptions</c></returns>
        public ContactsListOptions SortById(Order order)
        {
            AddSortCriteria(new Sort { Field = "id", Order = order });
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by label.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of <c>ContactsListOptions</c></returns>
        public ContactsListOptions SortByLabel(Order order)
        {
            AddSortCriteria(new Sort { Field = "label", Order = order });
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by email.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of <c>ContactsListOptions</c></returns>
        public ContactsListOptions SortByEmail(Order order)
        {
            AddSortCriteria(new Sort { Field = "email", Order = order });
            return this;
        }
    }
}