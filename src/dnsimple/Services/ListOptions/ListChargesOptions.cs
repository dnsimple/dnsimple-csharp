namespace dnsimple.Services.ListOptions
{
    /// <summary>
    /// Defines the options you may want to send to list charges,
    /// such as pagination and sorting
    /// </summary>
    /// <see cref="ListOptionsWithFiltering"/>
    public class ListChargesOptions : ListOptionsWithFiltering
    {
        /// <summary>
        /// Sets the start date to be filtered by.
        /// </summary>
        /// <param date="date">The start date we want to filter by.</param>
        /// <returns>The instance of the <c>ChargesListOptions</c></returns>
        public ListChargesOptions FilterByStartDate(string date)
        {
            AddFilter(new Filter { Field = "start_date", Value = date });
            return this;
        }

        /// <summary>
        /// Sets the end date to be filtered by.
        /// </summary>
        /// <param date="date">The end date we want to filter by.</param>
        /// <returns>The instance of the <c>ChargesListOptions</c></returns>
        public ListChargesOptions FilterByEndDate(string date)
        {
            AddFilter(new Filter { Field = "end_date", Value = date });
            return this;
        }
        
        /// <summary>
        /// Sets the order by which to sort by invoice date.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of <c>ChargesListOptions</c></returns>
        public ListChargesOptions SortByInvoiceDate(Order order)
        {
            AddSortCriteria(new Sort { Field = "invoiced", Order = order });
            return this;
        }
    }
}
