namespace dnsimple.Services.ListOptions
{
    /// <summary>
    /// Defines the options you may want to send to list domains, such as
    /// pagination, sorting and filtering.
    /// </summary>
    /// <see cref="ListOptionsWithFiltering"/>
    public class DomainListOptions : ListOptionsWithFiltering
    {
        private const string NameLikeFilter = "name_like";
        private const string RegistrantIdFilter = "registrant_id";

        private const string IdSort = "id";
        private const string NameSort = "name";
        private const string ExpiresOnSort = "expires_on";

        /// <summary>
        /// Creates a new instance of <c>DomainListOptions</c>.
        /// </summary>
        public DomainListOptions()
           => Pagination = new Pagination();
        
        /// <summary>
        /// Sets the order by which to sort by id.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>DomainListOptions</c></returns>
        public DomainListOptions SortById(Order order)
        {
            AddSortCriteria(new Sort { Field = IdSort, Order = order });
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by name.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>DomainListOptions</c></returns>
        public DomainListOptions SortByName(Order order)
        {
            AddSortCriteria(new Sort { Field = NameSort, Order = order });
            return this;
        }

        /// <summary>
        /// Sets the order by which to sort by expires on.
        /// </summary>
        /// <param name="order">The order in which we want to sort (asc or desc)</param>
        /// <returns>The instance of the <c>DomainListOptions</c></returns>
        public DomainListOptions SortByExpiresOn(Order order)
        {
            AddSortCriteria(new Sort { Field = ExpiresOnSort, Order = order });
            return this;
        }

        /// <summary>
        /// Sets the name to be filtered by.
        /// </summary>
        /// <param name="name">The name we want to filter by.</param>
        /// <returns>The instance of the <c>DomainListOptions</c></returns>
        public DomainListOptions FilterByName(string name)
        {
            AddFilter(new Filter { Field = NameLikeFilter, Value = name });
            return this;
        }

        /// <summary>
        /// Sets the registrant id to be filtered by.
        /// </summary>
        /// <param name="registrantId">The registrant id we want to filter by.</param>
        /// <returns>The instance of the <c>DomainListOptions</c></returns>
        public DomainListOptions FilterByRegistrantId(long registrantId)
        {
            AddFilter(new Filter { Field = RegistrantIdFilter, 
                Value = registrantId.ToString() });
            
            return this;
        }
    }
}