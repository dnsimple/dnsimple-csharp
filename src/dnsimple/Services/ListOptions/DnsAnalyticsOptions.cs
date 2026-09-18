using System.Collections.Generic;

namespace dnsimple.Services.ListOptions
{
    /// <summary>
    /// Defines the options you may want to send to query DNS Analytics, such
    /// as pagination, sorting, filtering and grouping.
    /// </summary>
    /// <see cref="ListOptionsWithFiltering"/>
    public class DnsAnalyticsOptions : ListOptionsWithFiltering
    {
        private IList<string> Groupings { get; } = new List<string>();

        public override bool HasFilterOptions()
        {
            return base.HasFilterOptions() || Groupings.Count > 0;
        }

        /// <summary>
        /// Unpacks the filters and the groupings into <c>KeyValuePair</c> objects.
        /// </summary>
        /// <returns>A list of KeyValuePair objects</returns>
        public override List<KeyValuePair<string, string>> UnpackFilters()
        {
            var filters = base.UnpackFilters();
            if (Groupings.Count > 0)
                filters.Add(new KeyValuePair<string, string>("groupings", string.Join(",", Groupings)));

            return filters;
        }

        /// <summary>
        /// Sets the start date (ISO8601) to be filtered by.
        /// </summary>
        /// <param name="date">The start date we want to filter by.</param>
        /// <returns>The instance of the <c>DnsAnalyticsOptions</c></returns>
        public DnsAnalyticsOptions FilterByStartDate(string date)
        {
            AddFilter(new Filter { Field = "start_date", Value = date });
            return this;
        }

        /// <summary>
        /// Sets the end date (ISO8601) to be filtered by.
        /// </summary>
        /// <param name="date">The end date we want to filter by.</param>
        /// <returns>The instance of the <c>DnsAnalyticsOptions</c></returns>
        public DnsAnalyticsOptions FilterByEndDate(string date)
        {
            AddFilter(new Filter { Field = "end_date", Value = date });
            return this;
        }

        /// <summary>
        /// Groups the results by date.
        /// </summary>
        /// <returns>The instance of the <c>DnsAnalyticsOptions</c></returns>
        public DnsAnalyticsOptions GroupByDate()
        {
            Groupings.Add("date");
            return this;
        }

        /// <summary>
        /// Groups the results by volume.
        /// </summary>
        /// <returns>The instance of the <c>DnsAnalyticsOptions</c></returns>
        public DnsAnalyticsOptions GroupByVolume()
        {
            Groupings.Add("volume");
            return this;
        }

        /// <summary>
        /// Groups the results by zone name.
        /// </summary>
        /// <returns>The instance of the <c>DnsAnalyticsOptions</c></returns>
        public DnsAnalyticsOptions GroupByZoneName()
        {
            Groupings.Add("zone_name");
            return this;
        }

        /// <summary>
        /// Sort results by date.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>DnsAnalyticsOptions</c></returns>
        /// <see cref="Order"/>
        public DnsAnalyticsOptions SortByDate(Order order)
        {
            AddSortCriteria(new Sort { Field = "date", Order = order });
            return this;
        }

        /// <summary>
        /// Sort results by volume.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>DnsAnalyticsOptions</c></returns>
        /// <see cref="Order"/>
        public DnsAnalyticsOptions SortByVolume(Order order)
        {
            AddSortCriteria(new Sort { Field = "volume", Order = order });
            return this;
        }

        /// <summary>
        /// Sort results by zone name.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>DnsAnalyticsOptions</c></returns>
        /// <see cref="Order"/>
        public DnsAnalyticsOptions SortByZoneName(Order order)
        {
            AddSortCriteria(new Sort { Field = "zone_name", Order = order });
            return this;
        }
    }
}
