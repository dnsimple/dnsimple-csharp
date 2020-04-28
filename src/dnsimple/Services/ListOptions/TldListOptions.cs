namespace dnsimple.Services.ListOptions
{
    public class TldListOptions : ListOptions
    {
        private const string TldSort = "tld";

        public TldListOptions SortByTld(Order order)
        {
            AddSortCriteria(new Sort { Field = TldSort, Order = order});
            return this;
        }
    }
}