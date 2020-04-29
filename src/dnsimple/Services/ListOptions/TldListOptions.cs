namespace dnsimple.Services.ListOptions
{
    /// <summary>
    /// Defines the options you may want to send to list TLDs, such as
    /// pagination and sorting.
    /// </summary>
    /// <see cref="ListOptions"/>
    public class TldListOptions : ListOptions
    {
        /// <summary>
        /// Sets the order by which to sort by TLD.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of <c>TldListOptions</c></returns>
        public TldListOptions SortByTld(Order order)
        {
            AddSortCriteria(new Sort { Field = "tld", Order = order});
            return this;
        }
    }
}